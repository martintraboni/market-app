using Models;
using Minimarket.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Data
{
    public static class CashCloseRepository
    {
        public static List<CashMovementListDto> GetMovimientosDelDia(DateTime fecha)
        {
            using var db = new MinimarketContext();
            var inicio = fecha.Date;
            var fin = fecha.Date.AddDays(1);

            return db.MovimientosCaja
                .Where(m => m.DateTime >= inicio && m.DateTime < fin)
                .OrderBy(m => m.DateTime)
                .Select(m => new CashMovementListDto
                {
                    Id = m.Id,
                    DateTime = m.DateTime,
                    Type = m.Type,
                    Amount = m.Amount,
                    Concept = m.Concept,
                    User = m.User.Username
                })
                .ToList();
        }

        public static List<CashCloseListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.CierresCaja
                .OrderByDescending(c => c.Date)
                .Select(c => new CashCloseListDto
                {
                    Id = c.Id,
                    Date = c.Date,
                    User = c.User.Username,
                    CashInHand = c.CashInHand,
                    PosTotal = c.PosTotal,
                    SystemTotal = c.SystemTotal,
                    Difference = c.Difference,
                    Notes = c.Notes
                })
                .ToList();
        }

        public static void Create(CashClose cashClose)
        {
            using var db = new MinimarketContext();
            db.CierresCaja.Add(cashClose);
            db.SaveChanges();

            // Registrar en auditoría
            db.Auditoria.Add(new AuditLog
            {
                UserId = cashClose.UserId,
                DateTime = DateTime.Now,
                Event = Constants.AuditEventCashClose,
                Details = $"Fecha: {cashClose.Date:dd/MM/yyyy} - Efectivo: {cashClose.CashInHand:C2} - Total Sistema: {cashClose.SystemTotal:C2} - Diferencia: {cashClose.Difference:C2}"
            });
            db.SaveChanges();
        }
        
        public static CashClose? GetById(int id)
        {
            using var db = new MinimarketContext();
            return db.CierresCaja
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);
        }

        public static decimal GetSaldoSistemaDia(DateTime fecha)
        {
            using var db = new MinimarketContext();
            var inicio = fecha.Date;
            var fin = fecha.Date.AddDays(1);
            
            var ingresos = db.MovimientosCaja
                .Where(m => m.DateTime >= inicio && m.DateTime < fin && m.Type == Constants.CashMovementTypeIn)
                .Sum(m => (decimal?)m.Amount) ?? 0;
            
            var egresos = db.MovimientosCaja
                .Where(m => m.DateTime >= inicio && m.DateTime < fin && m.Type == Constants.CashMovementTypeOut)
                .Sum(m => (decimal?)m.Amount) ?? 0;
            
            return ingresos - egresos;
        }

        public static (decimal efectivo, decimal tarjeta, decimal qr) GetVentasPorMetodoPago(DateTime fecha)
        {
            using var db = new MinimarketContext();
            var inicio = fecha.Date;
            var fin = fecha.Date.AddDays(1);
            
            var ventas = db.Ventas
                .Where(v => v.DateTime >= inicio && v.DateTime < fin)
                .ToList();
            
            var efectivo = ventas.Where(v => v.PaymentMethod == "Efectivo").Sum(v => v.Total);
            var tarjeta = ventas.Where(v => v.PaymentMethod == "Tarjeta").Sum(v => v.Total);
            var qr = ventas.Where(v => v.PaymentMethod == "QR").Sum(v => v.Total);
            
            return (efectivo, tarjeta, qr);
        }

        public static (decimal efectivoReal, decimal tarjetaReal, decimal qrReal, string detalle) CalcularCierreAutomatico(DateTime fecha)
        {
            using var db = new MinimarketContext();
            var inicio = fecha.Date;
            var fin = fecha.Date.AddDays(1);
            
            // Obtener ventas por método de pago
            var ventas = db.Ventas
                .Where(v => v.DateTime >= inicio && v.DateTime < fin)
                .ToList();
            
            var ventasEfectivo = ventas.Where(v => v.PaymentMethod == "Efectivo").Sum(v => v.Total);
            var ventasTarjeta = ventas.Where(v => v.PaymentMethod == "Tarjeta").Sum(v => v.Total);
            var ventasQR = ventas.Where(v => v.PaymentMethod == "QR").Sum(v => v.Total);
            
            // Obtener egresos de caja (compras, gastos, etc.) - solo afectan al efectivo
            var egresosEfectivo = db.MovimientosCaja
                .Where(m => m.DateTime >= inicio && m.DateTime < fin && m.Type == Constants.CashMovementTypeOut)
                .Sum(m => (decimal?)m.Amount) ?? 0;
            
            // Obtener otros ingresos en efectivo (movimientos manuales de ingreso)
            var otrosIngresosEfectivo = db.MovimientosCaja
                .Where(m => m.DateTime >= inicio && m.DateTime < fin && 
                           m.Type == Constants.CashMovementTypeIn && 
                           m.SaleId == null) // Solo ingresos que NO son de ventas
                .Sum(m => (decimal?)m.Amount) ?? 0;
            
            // Calcular efectivo real = ventas efectivo + otros ingresos - egresos
            var efectivoReal = ventasEfectivo + otrosIngresosEfectivo - egresosEfectivo;
            
            // Tarjeta y QR no se ven afectados por egresos (no salen físicamente de la caja)
            var tarjetaReal = ventasTarjeta;
            var qrReal = ventasQR;
            
            // Generar detalle explicativo
            var detalle = $"📊 CÁLCULO AUTOMÁTICO:\n\n" +
                         $"💵 EFECTIVO:\n" +
                         $"  + Ventas en efectivo: {ventasEfectivo:C2}\n" +
                         (otrosIngresosEfectivo > 0 ? $"  + Otros ingresos: {otrosIngresosEfectivo:C2}\n" : "") +
                         (egresosEfectivo > 0 ? $"  - Egresos (compras, gastos): {egresosEfectivo:C2}\n" : "") +
                         $"  = EFECTIVO REAL: {efectivoReal:C2}\n\n" +
                         $"💳 TARJETA:\n" +
                         $"  Ventas con tarjeta: {tarjetaReal:C2}\n\n" +
                         $"📱 QR:\n" +
                         $"  Ventas con QR: {qrReal:C2}\n\n" +
                         $"💰 TOTAL EN CAJA: {(efectivoReal + tarjetaReal + qrReal):C2}";
            
            return (efectivoReal, tarjetaReal, qrReal, detalle);
        }
    }
}
