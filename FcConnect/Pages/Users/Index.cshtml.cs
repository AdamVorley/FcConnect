using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FcConnect.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<User> Users { get;set; } = default!;
        public string SvgContent { get; private set; }
        public string CurrentFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SuspendedCheckHidden { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            bool showSuspendedUsers = SuspendedCheckHidden;
            int userStatusId = Constants.StatusUserActive;

            var identityUser = await _userManager.GetUserAsync(User);
            var signedInUser = _context.User.Find(identityUser.Id);

            if (!showSuspendedUsers)
            {
                userStatusId = Constants.StatusUserActive;
            }
            else
            {
                userStatusId = Constants.StatusUserSuspended;
            }

            if (!String.IsNullOrEmpty(searchString))
            {                
                Users = await _context.User.Where(u => u.UserStatusId <= userStatusId).Where(u => u.Surname.Contains(searchString) || u.Forename.Contains(searchString) ||
                (u.Forename + " " + u.Surname).Contains(searchString)).Where(u => u != signedInUser).ToListAsync();
            }
            else
            {
                Users = await _context.User.Where(u => u != signedInUser).Where(u => u.UserStatusId <= userStatusId).ToListAsync();
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "users.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            //Users = await _context.User.Where(u => u != signedInUser).ToListAsync();
        }
    }
}
