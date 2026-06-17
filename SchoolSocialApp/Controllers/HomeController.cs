using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Data;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View(new HomeIndexViewModel());
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.SchoolClassId == null)
            {
                return View(new HomeIndexViewModel());
            }

            var userWithClass = await _context.Users
                .Include(u => u.SchoolClass)
                .ThenInclude(c => c!.ClassSetting)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithClass?.SchoolClass == null)
            {
                return View(new HomeIndexViewModel());
            }

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => p.ClassId == userWithClass.SchoolClassId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(new HomeIndexViewModel
            {
                UserClass = userWithClass.SchoolClass,
                ClassSetting = userWithClass.SchoolClass.ClassSetting,
                Posts = posts
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
