namespace Minimarket.DTOs
{
    public class SaleListDto
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public DateTime DateTime { get; set; }
    }
}