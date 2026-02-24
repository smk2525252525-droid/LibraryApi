
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApi.Models
{
    public class Category//one category many users
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
