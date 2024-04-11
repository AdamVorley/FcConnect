using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FcConnect.Utilities;

namespace FcConnect.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogEvent _logEvent;

        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, LogEvent logEvent)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logEvent = logEvent;
        }

        [BindProperty]
        public User EditUser { get; set; } = default!;
        public string SvgContent { get; set; }
        public bool EmailConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string click)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (click != HttpContext.Session.GetString("UserEditClick")) 
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            HttpContext.Session.Remove("UserEditClick");

            // add id of user being edited to session
            HttpContext.Session.SetString("UserEditing", id.ToString());

            var identityUser = await _userManager.FindByIdAsync(id);
            EmailConfirmed = identityUser.EmailConfirmed;

            var user =  await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "update_user.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            EditUser = user;
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(User edituser)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // check id returned from form against session
            string sessionValue = HttpContext.Session.GetString("UserEditing");

            var signedInUser = await _userManager.GetUserAsync(User);
            string userId = "Unknown";
            if (signedInUser != null) { userId = signedInUser.Id; }

            if (edituser.Id.ToString() != sessionValue) 
            {
                await _logEvent.Log("Session value mismatch - edit user", "The session value " + sessionValue + 
                    " did not match the user being edited " + edituser.Id + ". The id may have been manipulated by the user", -1, userId, "");
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            _context.User.Attach(edituser);
            _context.Entry(edituser).Property(u => u.Forename).IsModified = true;
            _context.Entry(edituser).Property(u => u.Surname).IsModified = true;
            _context.SaveChanges();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(EditUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
