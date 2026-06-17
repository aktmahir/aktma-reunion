using System.ComponentModel.DataAnnotations;

namespace SchoolSocialApp.Models
{
    public class ClassSetting
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }

        public bool IsPostingAllowed { get; set; } = true;

        public bool CanShareResources { get; set; } = true;

        [StringLength(200, ErrorMessage = "The description must be at most {1} characters long.")]
        public string Description { get; set; } = string.Empty;
    }
}
