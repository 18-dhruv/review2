using System.Text.Json;
using Review5.exception;
using Review5.model;

namespace Review5.service
{
    public class TariffService
    {
        private readonly Dictionary<
            ConnectionType,
            List<TariffSlab>> tariffs;

        public TariffService(string tariffFilePath)
        {
            if (!File.Exists(tariffFilePath))
            {
                throw new FileNotFoundException($"Tariff file not found: {tariffFilePath}");
            }

            using var fileStream = new FileStream(tariffFilePath,FileMode.Open, FileAccess.Read);

            using var bufferedStream = new BufferedStream(fileStream);

            var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            var tariffFile = JsonSerializer.Deserialize<TariffFile>(bufferedStream, options);

            if (tariffFile == null)
            {
                throw new InvalidReadingException("Unable to read tariffs.json");
            }

            tariffs = new Dictionary<ConnectionType, List<TariffSlab>>();

            foreach (var connection in tariffFile.ConnectionTypes)
            {
                if (!Enum.TryParse(connection.Type, true, out ConnectionType connectionType))
                {
                    throw new UnknownConnectionTypeException($"Unknown connection type: {connection.Type}");
                }

                tariffs[connectionType] = connection.Slabs.OrderBy(slab => slab.UpTo).ToList();
            }
        }

        public decimal CalculateBill(ConnectionType connectionType, int consumption)
        {
            if (consumption < 0)
            {
                throw new InvalidReadingException("Consumption cannot be negative.");
            }

            if (consumption == 0)
            {
                return 0;
            }

            if (!tariffs.TryGetValue(connectionType, out var slabs))
            {
                throw new UnknownConnectionTypeException($"No tariff found for {connectionType}");
            }

            decimal total = 0;

            int previousLimit = 0;

            foreach (var slab in slabs)
            {
                if (consumption <= previousLimit)
                {
                    break;
                }

                int unitsInSlab = Math.Min(consumption, slab.UpTo) - previousLimit;

                if (unitsInSlab > 0)
                {
                    total += unitsInSlab * slab.Rate;
                }

                previousLimit = slab.UpTo;

                if (consumption <= slab.UpTo)
                {
                    break;
                }
            }

            return total;
        }
    }
}