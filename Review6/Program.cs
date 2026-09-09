using Review5.model;
using Review5.service;

namespace Review5
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvFilePath = "/Users/apple/Desktop/snakeAndLadder/Review6/Review6/reading.csv";

            string tariffFilePath = "/Users/apple/Desktop/snakeAndLadder/Review6/Review6/tariffs.json";

            string outputDirectory = "/Users/apple/Desktop/snakeAndLadder/Review6/Review6/Logs";

            string exceptionLogPath = "/Users/apple/Desktop/snakeAndLadder/Review6/Review6/Logs/meter_exceptions.log";

            Directory.CreateDirectory(outputDirectory);

            try
            {
                using var dbContext = new MeterDbContext();
                
                var readingRepo = new Repository<int, Reading>(dbContext);

                var billRepo = new Repository<int, Bill>(dbContext);

                var importService = new ImportReadingCsv(csvFilePath, readingRepo, exceptionLogPath);

                importService.Import();

                var tariffService = new TariffService(tariffFilePath);

                var billService = new GenerateBillsService(readingRepo, billRepo, tariffService, outputDirectory);

                billService.GenerateBills();

                Console.WriteLine("Process completed successfully.");
            }
            catch (Exception ex)
            {
                File.AppendAllText(exceptionLogPath, $"{DateTime.Now}: {ex.Message}" + Environment.NewLine);

                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit..." );

            Console.ReadKey();
        }
    }
}