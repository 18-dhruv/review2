namespace Review5.model
{
    public class Bill : IEntity<int>
    {
        public int Id { get; set; }

        public string MeterId { get; set; } = string.Empty;

        public int Consumption { get; set; }

        public decimal Amount { get; set; }
    }
}