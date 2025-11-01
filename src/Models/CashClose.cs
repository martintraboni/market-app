

namespace Models
{
    public class CashClose
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public decimal CashInHand { get; set; }
        public decimal PosTotal { get; set; }
        public decimal SystemTotal { get; set; }
        public decimal Difference { get; set; }
        public string Notes { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
