namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public bool IsActive { get; set; }

        // Relaciones de navegación
        public Category Category { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}
