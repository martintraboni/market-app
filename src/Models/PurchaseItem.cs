namespace Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal Cost { get; set; }
        public decimal Subtotal { get; set; }

        // Relaciones de navegación
        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
    }
}
