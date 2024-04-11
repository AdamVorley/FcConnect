// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FcConnect.Models;
using FcConnect.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FcConnect.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogEvent _logEvent;



        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            FcConnect.Data.ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment, LogEvent logEvent)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logEvent = logEvent;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        [BindProperty]
        public List<IdentityRole> Roles { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Role {  get; set; }
            [Required(ErrorMessage = "Forename is required.")]
            [Display(Name = "Forename")]
            public string Forename { get; set; }
            [Required(ErrorMessage = "Surname is required.")]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

        }

        public string SvgContent { get; set; }
        public void GetUserRoles() 
        {
            Roles = _roleManager.Roles.ToList();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "add_user.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            // retrieve roles from the database for dropdown
            GetUserRoles();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                string userRole = Input.Role;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
                {
                    string signedInUserId = HttpContext.Session.GetString("SignedInUserId");
                    string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                    //audit
                    await _logEvent.Log("User Created", " A new user was added to the system by User Id: " + signedInUserId + ". The new User Id is: " + user.Id + ".", -1, signedInUserId, userIpAddress);

                    await _userManager.AddToRoleAsync(user, userRole);

                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("TermsAccepted", "false"));

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    try
                    {
                        await _emailSender.SendEmailAsync(Input.Email, "FcConnect Portal - Confirm your email",
        $"Dear {Input.Forename}, <br /><br />Welcome to the FcConnect portal.<br /><br />To activate your account, please confirm your email address by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br /><br />Kind regards,<br /><br />FcConnect");
                    }
                    catch (Exception ex) 
                    {
                        Log errorLog = new()
                        {
                            Description = ex.Message.ToString(),
                            Name = "Email Send Failure",
                            Type = -1,
                            IpAddress = "",
                            SignedInUserId = ""

                        };
                        _context.Log.Add(errorLog);
                        await _context.SaveChangesAsync();
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);

                    }

                    int roleId = Constants.RoleUser;
                    if (userRole == "User") { roleId = Constants.RoleUser; }
                    if (userRole == "Admin") { roleId = Constants.RoleAdmin; }
                    if (userRole == "Developer") { roleId = Constants.RoleDeveloper; }

                    User newUser = new()
                    {
                        Id = userId,
                        Forename = Input.Forename,
                        Surname = Input.Surname,
                        Email = Input.Email,
                        RoleId = roleId,
                        UserStatusId = Constants.StatusUserActive
                    };                    

                    await _context.User.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToPage("Success");
            }

            // If we got this far, something failed, redisplay form
            GetUserRoles(); // call get user roles again to avoid null reference error when page reloads
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
