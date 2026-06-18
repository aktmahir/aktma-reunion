using System.ComponentModel.DataAnnotations;

namespace SchoolSocialApp.Models;

public class CreateClassViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The name must be at most {1} characters long.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "The description must be at most {1} characters long.")]
    public string Description { get; set; } = string.Empty;

    public string? SelectedUserId { get; set; }
    public IEnumerable<ApplicationUser> AvailableUsers { get; set; } = Enumerable.Empty<ApplicationUser>();
}

public class EditClassViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The name must be at most {1} characters long.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "The description must be at most {1} characters long.")]
    public string Description { get; set; } = string.Empty;
    public IEnumerable<ApplicationUser> Members { get; set; } = Enumerable.Empty<ApplicationUser>();
}