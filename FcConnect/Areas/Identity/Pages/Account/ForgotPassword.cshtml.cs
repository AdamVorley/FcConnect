#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FcConnect.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FcConnect.Data.ApplicationDbContext _context;



        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, FcConnect.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public string SvgContent { get; set; }

        public void OnGet() 
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "password_reset.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var userName = await _context.User.FindAsync(user.Id);
                string userForename = "User";

                if (userName != null) 
                {
                    userForename = userName.Forename;
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "FcConnect - Reset Password",
                    $"Dear {userForename}, <br /><br> />A passowrd reset request has been made for your FcConnect account." +
                    $"<br /><br />Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br /><br />" +
                    $"Kind regards,<br /><br />FcConnect");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
