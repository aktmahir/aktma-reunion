namespace SchoolSocialApp.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ClassSetting? ClassSetting { get; set; }
    }
}
