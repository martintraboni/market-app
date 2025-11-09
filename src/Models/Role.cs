namespace Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;

        // Relación de navegación
        public ICollection<User> Users { get; set; }
    }
}
