namespace Minimarket.DTOs
{
    public class CashMovementListDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Concept { get; set; }
        public string User { get; set; }
        public DateTime DateTime { get; set; }
    }
}