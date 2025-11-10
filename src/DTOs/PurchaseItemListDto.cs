namespace Minimarket.DTOs
{
    public class PurchaseItemListDto
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int Qty { get; set; }
        public decimal Cost { get; set; }
        public decimal Subtotal { get; set; }
    }
}