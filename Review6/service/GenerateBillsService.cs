using System.Text.Json;
using Review5.exception;
using Review5.model;

namespace Review5.service
{
    public class GenerateBillsService
    {
        private readonly IRepository<int, Reading> readingRepo;
        private readonly IRepository<int, Bill> billRepo;
        private readonly string outputDirectory;
        private readonly TariffService tariffService;

        public GenerateBillsService(
            IRepository<int, Reading> readingRepo,
            IRepository<int, Bill> billRepo,
            TariffService tariffService,
            string outputDirectory = ".")
        {
            this.readingRepo = readingRepo;
            this.billRepo = billRepo;
            this.tariffService = tariffService;
            this.outputDirectory =
                outputDirectory ?? ".";
        }

        public void GenerateBills()
        {
            Directory.CreateDirectory(outputDirectory);

            var readings = readingRepo.GetAll();

            var bills = new List<Bill>();

            foreach (var reading in readings)
            {
                try
                {
                    if (reading.PrevReading < 0 || reading.CurrReading < 0)
                    {
                        throw new InvalidReadingException($"Invalid reading for meter " + $"{reading.MeterId}");
                    }

                    if (reading.CurrReading < reading.PrevReading)
                    {
                        throw new MeterRollbackException($"Meter {reading.MeterId}: Current reading " + $"{reading.CurrReading} is less than previous " + $"reading {reading.PrevReading}");
                    }

                    int consumption = reading.CurrReading - reading.PrevReading;

                    using var memoryStream = new MemoryStream();

                    using (var writer = new BinaryWriter(memoryStream, System.Text.Encoding.UTF8, true))
                    { 
                        writer.Write(consumption);
                        writer.Flush();
                        
                    }

                    memoryStream.Position = 0;

                    int stagedConsumption;

                    using (var binaryReader = new BinaryReader(memoryStream, System.Text.Encoding.UTF8, true))
                    {
                        stagedConsumption = binaryReader.ReadInt32();
                    }
                    decimal amount = tariffService.CalculateBill(reading.ConnectionType, stagedConsumption);

                    var bill = new Bill { MeterId = reading.MeterId, Consumption = stagedConsumption, Amount = amount };

                    billRepo.Add(bill);

                    bills.Add(bill);

                    Console.WriteLine($"Meter {reading.MeterId}: " + $"Consumption = {stagedConsumption}, " + $"Amount = {amount}");
                }
                catch (EnergyException ex)
                {
                    LogException($"Meter {reading.MeterId}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    LogException($"Meter {reading.MeterId}: {ex.Message}");
                }
            }

            WriteJson(bills);
        }

        private void WriteJson(List<Bill> bills)
        {
            string outputPath = Path.Combine(outputDirectory, "energy_bills.json");

            var options =new JsonSerializerOptions { WriteIndented = true };

            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            using var bufferedStream = new BufferedStream(fileStream);

            JsonSerializer.Serialize(bufferedStream, bills, options);

            Console.WriteLine($"Bills exported successfully to: {outputPath}");
        }

        private void LogException(string message)
        {
            string path = Path.Combine(outputDirectory, "meter_exceptions.log");

            File.AppendAllText(path, $"{DateTime.Now}: {message}" + Environment.NewLine);
        }
    }
}