namespace Minimarket.Models
{
    public class Product
    {
        public int IDProducto { get; set; }
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Categoria { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMin { get; set; }
    }
}
