using Review5.exception;
using Review5.model;

namespace Review5.service
{
    public class ImportReadingCsv
    {
        private readonly string filePath;
        private readonly IRepository<int, Reading> repo;
        private readonly string exceptionLogPath;

        public ImportReadingCsv(
            string filePath,
            IRepository<int, Reading> repo,
            string exceptionLogPath = "meter_exceptions.log")
        {
            this.filePath = filePath;
            this.repo = repo;
            this.exceptionLogPath = exceptionLogPath;
        }

        public void Import()
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidReadingException($"CSV file not found: {filePath}");
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            using var bufferedStream = new BufferedStream(fileStream);

            using var reader = new StreamReader(bufferedStream);

            string? header = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(header))
            {
                throw new InvalidReadingException("CSV file is empty or contains no header.");
            }

            var processedMeters = new HashSet<string>();

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    ProcessLine(line, processedMeters);
                }
                catch (EnergyException ex)
                {
                    LogException(ex.Message);

                    Console.WriteLine($"Skipped invalid record: {ex.Message}");
                }
                catch (Exception ex)
                {
                    LogException(ex.Message);

                    Console.WriteLine($"Skipped invalid record: {ex.Message}");
                }
            }
        }

        private void ProcessLine(string line, HashSet<string> processedMeters)
        {
            string[] parts = line.Split(',');

            if (parts.Length != 4)
            {
                throw new InvalidReadingException($"Invalid CSV row: {line}");
            }

            string meterId = parts[0].Trim();

            if (string.IsNullOrWhiteSpace(meterId))
            {
                throw new InvalidReadingException("MeterId cannot be empty.");
            }

            if (!int.TryParse(parts[1].Trim(), out int prevReading))
            {
                throw new InvalidReadingException($"Invalid previous reading for meter {meterId}");
            }

            if (!int.TryParse(parts[2].Trim(), out int currReading))
            {
                throw new InvalidReadingException($"Invalid current reading for meter {meterId}");
            }

            if (!Enum.TryParse(parts[3].Trim(), true, out ConnectionType connectionType))
            {
                throw new UnknownConnectionTypeException($"Unknown connection type: {parts[3].Trim()}");
            }

            if (processedMeters.Contains(meterId))
            {
                throw new DuplicateMeterException($"Duplicate MeterId found: {meterId}");
            }

            if (prevReading < 0 || currReading < 0)
            {
                throw new InvalidReadingException($"Reading cannot be negative for meter {meterId}");
            }

            if (currReading < prevReading)
            {
                throw new MeterRollbackException($"Meter {meterId}: Current reading {currReading} " + $"is less than previous reading {prevReading}");
            }

            var reading = new Reading(meterId, prevReading, currReading, connectionType);

            repo.Add(reading);

            processedMeters.Add(meterId);

            Console.WriteLine($"Meter {meterId} imported successfully.");
        }

        private void LogException(string message)
        {
            File.AppendAllText(exceptionLogPath, $"{DateTime.Now}: {message}" + Environment.NewLine);
        }
    }
}