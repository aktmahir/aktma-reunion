using System.ComponentModel.DataAnnotations;

namespace SchoolSocialApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The message must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
