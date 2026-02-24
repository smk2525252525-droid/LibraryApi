
using System.ComponentModel.DataAnnotations;


namespace LibraryApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } // Keep required, as Name is essential

        [Required]
        [EmailAddress]
        public required string Email { get; set; } // Keep required

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }

        // REMOVED 'required' and made nullable so the compiler doesn't force initialization
        public virtual Role? Role { get; set; }

        // REMOVED 'required'. The default value '= new List<Issuing>()' is enough.
        public virtual ICollection<Issuing> Issuings { get; set; } = new List<Issuing>();
    }
}