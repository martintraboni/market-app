namespace Minimarket.Models
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int IdSale { get; set; }
        public int IdProduct { get; set; }
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public Product Producto { get; set; }
        public Sale Venta { get; set; }
    }
}
