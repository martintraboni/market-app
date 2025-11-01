namespace Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Qty { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int? RefId { get; set; }

        // Relación de navegación
        public Product Product { get; set; }
    }
}
