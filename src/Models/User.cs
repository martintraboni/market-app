namespace Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relaciones de navegación
        public ICollection<Sale> Sales { get; set; }
    }
}
