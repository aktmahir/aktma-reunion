using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Data;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.SchoolClassId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var schoolClass = await _context.SchoolClasses
                .Include(c => c.ClassSetting)
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == user.SchoolClassId.Value);

            return View(schoolClass);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.SchoolClassId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var setting = await _context.ClassSettings
                .FirstOrDefaultAsync(s => s.ClassId == user.SchoolClassId.Value);

            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(int id, ClassSetting model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.SchoolClassId != model.ClassId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var setting = await _context.ClassSettings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }

            setting.IsPostingAllowed = model.IsPostingAllowed;
            setting.CanShareResources = model.CanShareResources;
            setting.Description = model.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
