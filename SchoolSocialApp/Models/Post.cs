namespace SchoolSocialApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
