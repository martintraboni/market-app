namespace Models
{
    public class CashMovement
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = string.Empty; // IN/OUT
        public decimal Amount { get; set; }
        public string Concept { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? SaleId { get; set; }

        // Relaciones de navegación
        public User User { get; set; }
        public Sale Sale { get; set; }
    }
}
