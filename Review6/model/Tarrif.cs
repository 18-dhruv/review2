namespace Review5.model
{
    public class TariffFile
    {
        public List<ConnectionTariff> ConnectionTypes { get; set; } = new();
    }

    public class ConnectionTariff
    {
        public string Type { get; set; } = string.Empty;

        public List<TariffSlab> Slabs { get; set; } = new();
    }

    public class TariffSlab
    {
        public int UpTo { get; set; }

        public decimal Rate { get; set; }
    }
}