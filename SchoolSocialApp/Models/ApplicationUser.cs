using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SchoolSocialApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100, ErrorMessage = "The full name must be at most {1} characters long.")]
        public string FullName { get; set; } = string.Empty;

        public int? SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
