using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Data;

namespace SchoolSocialApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.SchoolClasses
                .Include(c => c.Members)
                .Include(c => c.ClassSetting)
                .ToListAsync();

            return View(classes);
        }
    }
}
