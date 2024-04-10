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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FcConnect.Pages.Users
{
    public class SuspendUserModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SuspendUserModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        //  [BindProperty]
        public User User { get; set; } = default!;
        public string SvgContent { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "suspend.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string id)
        {
            /* if (!ModelState.IsValid)
             {
                 return Page();
             }*/

            var identityUser = await _userManager.FindByIdAsync(id);
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);

            var userClaims = await _userManager.GetClaimsAsync(identityUser);
            var existingClaim = userClaims.FirstOrDefault(r => r.Type == "UserSuspended");

            if (user.UserStatusId == Constants.StatusUserActive) 
            {
                user.UserStatusId = Constants.StatusUserSuspended;

                if (existingClaim != null)
                {
                    await _userManager.ReplaceClaimAsync(identityUser, existingClaim, new Claim("UserSuspended", "true"));
                }
                else 
                {
                    await _userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim("UserSuspended", "true"));
                }
            }
            else if (user.UserStatusId == Constants.StatusUserSuspended) 
            {
                user.UserStatusId = Constants.StatusUserActive;
                await _userManager.ReplaceClaimAsync(identityUser, existingClaim, new Claim("UserSuspended", "false"));
            }

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
