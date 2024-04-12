// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
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
using Microsoft.IdentityModel.Tokens;

namespace FcConnect.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     
        ///     
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     
        ///     
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     
            ///     
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public string QueryEmail {  get; set; }
        public string SvgContent { get; set; }  

        public void OnGet(string email)
        {
            if (!email.IsNullOrEmpty())
            {
                QueryEmail = email;
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "resend.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "resend.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var userName = await _context.User.FindAsync(userId);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.Email,
                "FcConnect Portal - Confirm your email",
                $"Dear {userName.Forename}, <br /><br />Welcome to the FcConnect portal.<br /><br />To activate your account, please confirm your email address by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br /><br />Kind regards,<br /><br />FcConnect");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
