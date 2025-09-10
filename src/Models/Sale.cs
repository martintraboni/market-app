using System;
using System.Collections.Generic;

namespace Minimarket.Models
{
    public class Sale
    {
        public int IDVenta { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string MedioPago { get; set; } = "Efectivo";
        public List<SaleItem> Items { get; set; } = new();
    }

    public class SaleItem
    {
        public int IDDetalle { get; set; }
        public int IDVenta { get; set; }
        public int IDProducto { get; set; }
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
