using FcConnect.Models;
using FcConnect.Utilities;
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
        private readonly LogEvent _logEvent;


        public IndexModel(UserManager<IdentityUser> userManager, FcConnect.Data.ApplicationDbContext context, LogEvent logEvent) 
        {
            _userManager = userManager;
            _context = context;
            _logEvent = logEvent;
        }

        public async Task<IActionResult> OnPostAgreeAsync()
        {
            // update user claim to terms accepted
            var user = await _userManager.GetUserAsync(User);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var existingClaim = userClaims.FirstOrDefault(r => r.Type == "TermsAccepted");

            await _userManager.ReplaceClaimAsync(user, existingClaim, new Claim("TermsAccepted", "true"));

            // audit user has accepted T&Cs
            await _logEvent.LogEvent("Terms Accepted by User", "User id: " + user.Id + " accepted the application T&Cs", -1, user.Id, "");           

            return RedirectToPage("./TermsAccepted");
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // audit user has opened T&Cs
            await _logEvent.LogEvent("Terms Page Opened", "User id: " + user.Id + " viewed the application T&Cs", -1, user.Id, "");

        }
    }
}
