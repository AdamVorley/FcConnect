using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FcConnect.Areas.Identity.Pages.Terms
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FcConnect.Data.ApplicationDbContext _context;


        public IndexModel(UserManager<IdentityUser> userManager, FcConnect.Data.ApplicationDbContext context) 
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnPostAgreeAsync()
        {
            // update user claim to terms accepted
            var user = await _userManager.GetUserAsync(User);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var existingClaim = userClaims.FirstOrDefault(r => r.Type == "TermsAccepted");

            await _userManager.ReplaceClaimAsync(user, existingClaim, new Claim("TermsAccepted", "true"));

            // TODO - log
            Log logTermsAccepted = new()
            {
                Name = "Terms Accepted",
                Description = "Terms were accepted by the user",
                IpAddress = "",
                Type = -1,
                SignedInUserId = user.Id
            };

            _context.Log.Add(logTermsAccepted);
            await _context.SaveChangesAsync();

            return RedirectToPage("./TermsAccepted");
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Log logTermsOpened = new()
            {
                Name = "Terms Page Opened",
                Description = "The terms page was opened by the user",
                IpAddress = "",
                Type = -1,
                SignedInUserId = user.Id
            };

            _context.Log.Add(logTermsOpened);
            await _context.SaveChangesAsync();
        }
    }
}
