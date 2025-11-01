
namespace Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }

        // Relaciones de navegación
        public User User { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
        public ICollection<CashMovement> CashMovements { get; set; }
    }
}
