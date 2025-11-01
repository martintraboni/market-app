namespace Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime Date { get; set; }
        public string DocNumber { get; set; } = "";
        public decimal Total { get; set; }

        // Relación de navegación
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}
