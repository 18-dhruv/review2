namespace Review5.model
{
    public class Reading : IEntity<int>
    {
        public int Id { get; set; }

        public string MeterId { get; set; }

        public int PrevReading { get; set; }

        public int CurrReading { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public Reading()
        {
            MeterId = string.Empty;
        }

        public Reading(
            string meterId,
            int prevReading,
            int currReading,
            ConnectionType connectionType)
        {
            MeterId = meterId;
            PrevReading = prevReading;
            CurrReading = currReading;
            ConnectionType = connectionType;
        }
    }

    public enum ConnectionType
    {
        DOMESTIC,
        COMMERCIAL,
        SOLAR9
    }
}