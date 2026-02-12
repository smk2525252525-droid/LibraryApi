
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public required string Title { get; set; }

        [Required, MaxLength(150)]
        public required string Author { get; set; }

        // Change from 'string Category' to 'int CategoryId' to link tables
        public int CategoryId { get; set; }

        // Navigation property (Remove 'required')
        public virtual Category? Category { get; set; }

        [Required]
        public string Status { get; set; } = "Available"; 

        [JsonIgnore]
        public virtual ICollection<Issuing> Issuings { get; set; } = new List<Issuing>();
    }
}