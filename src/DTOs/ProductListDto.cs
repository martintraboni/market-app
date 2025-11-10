namespace Minimarket.DTOs
{
    public class ProductListDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public bool IsActive { get; set; }
    }
}
