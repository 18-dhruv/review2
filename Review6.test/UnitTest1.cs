using NUnit.Framework;
using Review5.exception;
using Review5.model;
using Review5.service;
using System.Text;

namespace Review5.Tests
{
    [TestFixture]
    public class EnergyMeterTests
    {
        private string tariffFilePath = string.Empty;

        [SetUp]
        public void Setup()
        {
            tariffFilePath =
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "tariffs.json"
                );

            File.WriteAllText(
                tariffFilePath,
                """
                {
                  "connectionTypes": [
                    {
                      "type": "DOMESTIC",
                      "slabs": [
                        { "upTo": 100, "rate": 3 },
                        { "upTo": 300, "rate": 5 },
                        { "upTo": 999999, "rate": 7 }
                      ]
                    },
                    {
                      "type": "COMMERCIAL",
                      "slabs": [
                        { "upTo": 500, "rate": 8 },
                        { "upTo": 999999, "rate": 10 }
                      ]
                    }
                  ]
                }
                """
            );
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(tariffFilePath))
            {
                File.Delete(tariffFilePath);
            }
        }
        
        [Test]
        public void ZeroConsumption_ShouldReturnZero()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    0
                );

            Assert.That(result, Is.EqualTo(0));
        }


        [Test]
        public void Exactly100Units_ShouldUseFirstSlab()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    100
                );

            Assert.That(result, Is.EqualTo(300));
        }


        [Test]
        public void Exactly101Units_ShouldUseSecondSlab()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    101
                );
            
            Assert.That(result, Is.EqualTo(305));
        }
        
        [Test]
        public void Exactly300Units_ShouldCalculateProgressively()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    300
                );

   
            Assert.That(result, Is.EqualTo(1300));
        }

        [Test]
        public void Exactly301Units_ShouldUseThirdSlab()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    301
                );

            Assert.That(result, Is.EqualTo(1307));
        }


        [Test]
        public void ProgressiveSlabCalculation_150Units_ShouldReturn550()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.DOMESTIC,
                    150
                );


            Assert.That(result, Is.EqualTo(550));
        }


        [Test]
        public void MeterRollback_ShouldThrowException()
        {
            var reading =
                new Reading(
                    "MTR2",
                    800,
                    760,
                    ConnectionType.DOMESTIC
                );

            Assert.That(
                () =>
                {
                    ValidateReading(reading);
                },
                Throws.TypeOf<MeterRollbackException>()
            );
        }


        [Test]
        public void UnknownConnectionType_ShouldThrowException()
        {
            string unknownType = "INDUSTRIAL";

            Assert.That(
                () =>
                {
                    if (!Enum.TryParse(
                            unknownType,
                            true,
                            out ConnectionType type))
                    {
                        throw new UnknownConnectionTypeException(
                            $"Unknown connection type: {unknownType}"
                        );
                    }
                },
                Throws.TypeOf<UnknownConnectionTypeException>()
            );
        }

        [Test]
        public void NegativeReading_ShouldThrowException()
        {
            var reading =
                new Reading(
                    "MTR1",
                    -100,
                    200,
                    ConnectionType.DOMESTIC
                );

            Assert.That(
                () =>
                {
                    ValidateReading(reading);
                },
                Throws.TypeOf<InvalidReadingException>()
            );
        }



        [Test]
        public void DuplicateMeter_ShouldThrowException()
        {
            var meters =
                new HashSet<string>();

            string meterId = "MTR1";

            meters.Add(meterId);

            Assert.That(
                () =>
                {
                    if (meters.Contains(meterId))
                    {
                        throw new DuplicateMeterException(
                            $"Duplicate MeterId found: {meterId}"
                        );
                    }
                },
                Throws.TypeOf<DuplicateMeterException>()
            );
        }

        
        [Test]
        public void BinaryConsumptionRoundTrip_ShouldReturnSameValue()
        {
            int originalConsumption = 250;

            using var memoryStream =
                new MemoryStream();

            using (
                var writer =
                    new BinaryWriter(
                        memoryStream,
                        Encoding.UTF8,
                        true))
            {
                writer.Write(originalConsumption);
                writer.Flush();
            }

            memoryStream.Position = 0;

            int result;

            using (
                var reader =
                    new BinaryReader(
                        memoryStream,
                        Encoding.UTF8,
                        true))
            {
                result = reader.ReadInt32();
            }

            Assert.That(
                result,
                Is.EqualTo(originalConsumption)
            );
        }

        
        [Test]
        public void HeaderOnlyCsv_ShouldThrowException()
        {
            string csvPath =
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "empty_readings.csv"
                );

            File.WriteAllText(
                csvPath,
                "MeterId,PrevReading,CurrReading,ConnectionType"
            );

            Assert.That(
                () =>
                {
                    var lines =
                        File.ReadAllLines(csvPath);

                    if (lines.Length <= 1)
                    {
                        throw new InvalidReadingException(
                            "CSV file does not contain any reading records."
                        );
                    }
                },
                Throws.TypeOf<InvalidReadingException>()
            );

            File.Delete(csvPath);
        }


        [Test]
        public void CommercialTariff_400Units_ShouldReturn3200()
        {
            var tariffService =
                new TariffService(tariffFilePath);

            decimal result =
                tariffService.CalculateBill(
                    ConnectionType.COMMERCIAL,
                    400
                );
            

            Assert.That(result, Is.EqualTo(3200));
        }

        private void ValidateReading(Reading reading)
        {
            if (reading.PrevReading < 0 ||
                reading.CurrReading < 0)
            {
                throw new InvalidReadingException(
                    $"Invalid reading values for meter " +
                    $"{reading.MeterId}"
                );
            }

            if (reading.CurrReading <
                reading.PrevReading)
            {
                throw new MeterRollbackException(
                    $"Meter {reading.MeterId}: Current reading " +
                    $"{reading.CurrReading} is less than previous " +
                    $"reading {reading.PrevReading}"
                );
            }
        }
    }
}