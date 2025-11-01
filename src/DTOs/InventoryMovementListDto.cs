namespace Minimarket.DTOs
{
    public class InventoryMovementListDto
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public string Type { get; set; }
        public int Qty { get; set; }
        public string Reason { get; set; }
        public DateTime DateTime { get; set; }
    }
}