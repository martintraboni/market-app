namespace Minimarket.DTOs
{
    public class AuditLogListDto
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Event { get; set; }
        public string Details { get; set; }
        public DateTime DateTime { get; set; }
    }
}