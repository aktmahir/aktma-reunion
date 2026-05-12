namespace SchoolSocialApp.Models
{
    public class HomeIndexViewModel
    {
        public SchoolClass? UserClass { get; set; }
        public ClassSetting? ClassSetting { get; set; }
        public IEnumerable<Post> Posts { get; set; } = Enumerable.Empty<Post>();
    }
}
