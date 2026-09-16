namespace Entities
{
    public class Person
    {
        // Primary Key
        public int Id { get; set; }

        // Basic Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // This is unique and essential for bank records
        public string NationalId { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties (Relationships)
        // These are virtual to allow EF Core to use "Lazy Loading" if needed
        public int CreatedByUserID { get; set; }
        public virtual User User { get; set; }
    
    }
}
