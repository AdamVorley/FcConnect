using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FcConnect.Areas.Identity.Pages.Terms
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAgreeAsync()
        {
            // update user claim to terms accepted
            var user = await _userManager.GetUserAsync(User);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var existingClaim = userClaims.FirstOrDefault(r => r.Type == "TermsAccepted");

            await _userManager.ReplaceClaimAsync(user, existingClaim, new Claim("TermsAccepted", "true"));

            // TODO - log

            return RedirectToPage("/Index");
        }

        public void OnGet()
        {
        }
    }
}
