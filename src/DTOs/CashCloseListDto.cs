namespace Minimarket.DTOs
{
    public class CashCloseListDto
    {
        public int Id { get; set; }
        public string User { get; set; }
        public decimal CashInHand { get; set; }
        public decimal PosTotal { get; set; }
        public decimal SystemTotal { get; set; }
        public decimal Difference { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
    }
}