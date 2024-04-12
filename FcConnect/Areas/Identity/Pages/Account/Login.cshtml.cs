// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FcConnect.Utilities;

namespace FcConnect.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly LogEvent _logEvent;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, IWebHostEnvironment webHostEnvironment, LogEvent logEvent)
        {
            _signInManager = signInManager;
            _logEvent = logEvent;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     
        ///     
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     
        ///     
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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

            /// <summary>
            ///     
            ///     
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     
            ///     
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

        }
        public string SvgContent { get; private set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "family.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await _logEvent.Log("New sign in", "User " + Input.Email + " signed in", -1, "", "");

                    // check if user has accepted terms
                   if (User.HasClaim(c => c.Type == "TermsAccepted" && c.Value == "false")) 
                   {
                        await _logEvent.Log("Terms not yet accepted sign in", "User " + Input.Email + " signed in and was redirected to terms page", -1, "", "");

                        return LocalRedirect("~/Terms");
                   }

                    // Check if user is suspended
                    if (User.HasClaim(c => c.Type == "UserSuspended" && c.Value == "true"))
                    {
                        await _logEvent.Log("Attempted sign in - suspended user", "User " + Input.Email + " attempted to sign in", -1, "", "");

                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "Account suspended.");
                        return Page(); 
                    }

                    return LocalRedirect(returnUrl);
                }
                var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "family.svg");
                SvgContent = System.IO.File.ReadAllText(svgFilePath);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    await _logEvent.Log("Failed sign in", "User " + Input.Email + " attempted to sign in", -1, "", "");

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
