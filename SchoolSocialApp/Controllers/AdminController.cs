using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSocialApp.Data;
using SchoolSocialApp.Models;

namespace SchoolSocialApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.SchoolClasses
                .Include(c => c.Members)
                .Include(c => c.ClassSetting)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> CreateClass()
        {
            var users = await _userManager.Users.Where(u => u.SchoolClassId == null).ToListAsync();
            var model = new CreateClassViewModel { AvailableUsers = users };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(CreateClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableUsers = await _userManager.Users.Where(u => u.SchoolClassId == null).ToListAsync();
                return View(model);
            }

            var schoolClass = new SchoolClass
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.SchoolClasses.Add(schoolClass);
            await _context.SaveChangesAsync();

            _context.ClassSettings.Add(new ClassSetting
            {
                ClassId = schoolClass.Id,
                IsPostingAllowed = true,
                CanShareResources = true,
                Description = "New class settings"
            });

            if (model.SelectedUserId != null)
            {
                var user = await _userManager.FindByIdAsync(model.SelectedUserId);
                if (user != null)
                {
                    user.SchoolClassId = schoolClass.Id;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditClass(int id)
        {
            var schoolClass = await _context.SchoolClasses
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (schoolClass == null)
            {
                return NotFound();
            }

            var model = new EditClassViewModel
            {
                Id = schoolClass.Id,
                Name = schoolClass.Name,
                Description = schoolClass.Description,
                Members = schoolClass.Members.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(EditClassViewModel model)
        {
            var schoolClass = await _context.SchoolClasses.FirstOrDefaultAsync(c => c.Id == model.Id);
            if (schoolClass == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Members = (await _context.Users.Where(u => u.SchoolClassId == model.Id).ToListAsync());
                return View(model);
            }

            schoolClass.Name = model.Name;
            schoolClass.Description = model.Description;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
