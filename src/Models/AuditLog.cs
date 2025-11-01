namespace Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime DateTime { get; set; }
        public string Event { get; set; }
        public string Details { get; set; }
    }
}
