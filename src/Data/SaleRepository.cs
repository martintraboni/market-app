using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Minimarket.Models;

namespace Minimarket.Data
{
    public static class SaleRepository
    {
        public static int CrearVenta(Sale v)
        {
            using var cn = Db.GetConnection();
            cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                var cmdVenta = new SqlCommand(
                    "INSERT INTO Ventas (Fecha, Total, MedioPago) OUTPUT INSERTED.IDVenta VALUES (@Fecha, @Total, @MedioPago)",
                    cn, tx);
                cmdVenta.Parameters.AddWithValue("@Fecha", v.Fecha);
                cmdVenta.Parameters.AddWithValue("@Total", v.Total);
                cmdVenta.Parameters.AddWithValue("@MedioPago", v.MedioPago);
                int idVenta = (int)cmdVenta.ExecuteScalar();

                foreach (var it in v.Items)
                {
                    var cmdDet = new SqlCommand(
                        @"INSERT INTO DetalleVentas (IDVenta, IDProducto, Cantidad, PrecioUnitario)
                          VALUES (@IDVenta, @IDProducto, @Cantidad, @PrecioUnitario)", cn, tx);
                    cmdDet.Parameters.AddWithValue("@IDVenta", idVenta);
                    cmdDet.Parameters.AddWithValue("@IDProducto", it.IDProducto);
                    cmdDet.Parameters.AddWithValue("@Cantidad", it.Cantidad);
                    cmdDet.Parameters.AddWithValue("@PrecioUnitario", it.PrecioUnitario);
                    cmdDet.ExecuteNonQuery();

                    var cmdStock = new SqlCommand(
                        "UPDATE Productos SET Stock = Stock - @cant WHERE IDProducto=@id", cn, tx);
                    cmdStock.Parameters.AddWithValue("@cant", it.Cantidad);
                    cmdStock.Parameters.AddWithValue("@id", it.IDProducto);
                    cmdStock.ExecuteNonQuery();
                }

                tx.Commit();
                return idVenta;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public static DataTable ReporteVentasPorFecha(DateTime desde, DateTime hasta)
        {
            string sql = @"SELECT v.IDVenta, v.Fecha, v.Total, v.MedioPago
                           FROM Ventas v
                           WHERE v.Fecha BETWEEN @d1 AND @d2
                           ORDER BY v.Fecha DESC";
            return Db.Query(sql,
                new SqlParameter("@d1", desde),
                new SqlParameter("@d2", hasta));
        }
    }
}
