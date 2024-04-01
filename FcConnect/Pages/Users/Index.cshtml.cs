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


        public IndexModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<User> Users { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var signedInUser = _context.User.Find(identityUser.Id);

            Users = await _context.User.Where(u => u != signedInUser).ToListAsync();
        }
    }
}
