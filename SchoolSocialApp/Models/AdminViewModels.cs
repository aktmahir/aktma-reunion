namespace SchoolSocialApp.Models;

public class CreateClassViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SelectedUserId { get; set; }
    public IEnumerable<ApplicationUser> AvailableUsers { get; set; } = Enumerable.Empty<ApplicationUser>();
}

public class EditClassViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<ApplicationUser> Members { get; set; } = Enumerable.Empty<ApplicationUser>();
}