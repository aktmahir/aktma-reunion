using System.ComponentModel.DataAnnotations;

namespace SchoolSocialApp.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be at most {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "The description must be at most {1} characters long.")]
        public string Description { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ClassSetting? ClassSetting { get; set; }
    }
}
