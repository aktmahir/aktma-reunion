using Microsoft.AspNetCore.Identity;

namespace SchoolSocialApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public int? SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
