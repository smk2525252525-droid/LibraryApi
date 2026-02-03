namespace LibraryApi.Models
{
    public class Issuing
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        // Remove 'required' so we can create Issuings using just the ID
        public virtual User? User { get; set; }

        public int BookId { get; set; }
        public virtual Book? Book { get; set; }

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedAt { get; set; }
    }
}