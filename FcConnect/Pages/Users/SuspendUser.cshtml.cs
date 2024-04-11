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
using FcConnect.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class SuspendUserModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogEvent _logEvent;

        public SuspendUserModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, LogEvent logEvent)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logEvent = logEvent;
        }

        public User UserModel { get; set; } = default!;
        public string SvgContent { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signedInUser = await _userManager.GetUserAsync(User);
            string signedInUserId;

            if (signedInUser != null)
            {
                signedInUserId = signedInUser.Id.ToString();
            }
            else 
            {
                signedInUserId = "NotFound - User was null";
            }

            string sessionValue = HttpContext.Session.GetString("UserEditing");

            // ensure user has reached suspend page through user management
            if (id != sessionValue) 
            {
                string ip = HttpContext.Connection.RemoteIpAddress.ToString();
                await _logEvent.Log("Error Forbidden - Suspend User", 
                    "The session value for UserEditing was invalid - the user may have attempted to enter this page via URL injection. Session Value: " + sessionValue + ", Query String value: " + id, -1, signedInUserId, ip);

                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            var user =  await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            UserModel = user;

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "suspend.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string id)
        {
            
            var identityUser = await _userManager.FindByIdAsync(id);
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);

            var userClaims = await _userManager.GetClaimsAsync(identityUser);
            var existingClaim = userClaims.FirstOrDefault(r => r.Type == "UserSuspended");

            // get signed in user data for auditing
            var signedInUser = await _userManager.GetUserAsync(User);
            string signedInUserId;
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();


            if (signedInUser != null)
            {
                signedInUserId = signedInUser.Id.ToString();
            }
            else
            {
                signedInUserId = "NotFound - User was null";
            }

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
                // audit
                await _logEvent.Log("User Suspended", "User Id: " + user.Id + " was suspended by User Id: " + signedInUserId, -1, signedInUserId, ip);
            }
            else if (user.UserStatusId == Constants.StatusUserSuspended) 
            {
                user.UserStatusId = Constants.StatusUserActive;
                await _userManager.ReplaceClaimAsync(identityUser, existingClaim, new Claim("UserSuspended", "false"));

                //audit
                await _logEvent.Log("User Reinstated", "User Id: " + user.Id + " was reinstated by User Id: " + signedInUserId, -1, signedInUserId, ip);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(UserModel.Id))
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
