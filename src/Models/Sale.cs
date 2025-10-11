namespace Minimarket.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string MedioPago { get; set; } = "Efectivo";
        public List<SaleItem> Items { get; set; } = new();
    }

}
