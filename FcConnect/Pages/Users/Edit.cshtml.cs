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

namespace FcConnect.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        public string SvgContent { get; set; }

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

            var user =  await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "password_reset.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(User user)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // check id returned from form against session
            string sessionValue = HttpContext.Session.GetString("UserEditing");

            if (user.Id.ToString() != sessionValue) 
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            _context.User.Attach(user);
            _context.Entry(user).Property(u => u.Forename).IsModified = true;
            _context.Entry(user).Property(u => u.Surname).IsModified = true;
            _context.Entry(user).Property(u => u.Email).IsModified = true;
            _context.SaveChanges();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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
