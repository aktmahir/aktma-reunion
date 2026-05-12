namespace SchoolSocialApp.Models
{
    public class ClassSetting
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }
        public bool IsPostingAllowed { get; set; } = true;
        public bool CanShareResources { get; set; } = true;
        public string Description { get; set; } = string.Empty;
    }
}
