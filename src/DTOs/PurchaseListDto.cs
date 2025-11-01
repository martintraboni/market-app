namespace Minimarket.DTOs
{
    public class PurchaseListDto
    {
        public int Id { get; set; }
        public string Supplier { get; set; }
        public string DocNumber { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
    }
}