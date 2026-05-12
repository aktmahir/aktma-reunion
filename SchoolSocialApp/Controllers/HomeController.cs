using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Data;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View(new HomeIndexViewModel());
            }

            var user = await _context.Users
                .Include(u => u.SchoolClass)
                .ThenInclude(c => c.ClassSetting)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null || user.SchoolClassId == null)
            {
                return View(new HomeIndexViewModel());
            }

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => p.ClassId == user.SchoolClassId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(new HomeIndexViewModel
            {
                UserClass = user.SchoolClass,
                ClassSetting = user.SchoolClass?.ClassSetting,
                Posts = posts
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
