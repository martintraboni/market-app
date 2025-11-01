namespace Minimarket.DTOs
{
    public class SaleItemListDto
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}