namespace Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CUIT { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Relación de navegación
        public ICollection<Purchase> Purchases { get; set; }
    }
}
