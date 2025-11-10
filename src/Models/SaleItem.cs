
namespace Models
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string Code { get; set; }
        public string Descripcion { get; set; }

        // Relaciones de navegación
        public Sale Sale { get; set; }
        public Product Product { get; set; }
    }
}
