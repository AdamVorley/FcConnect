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

namespace FcConnect.Pages.Surveys.Assign
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        public string CurrentFilter { get; set; }


        public IndexModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                User = await _context.User.Where(u => u.RoleId == Constants.RoleUser).Where(u => u.Surname.Contains(searchString) || u.Forename.Contains(searchString) ||
                (u.Forename + " " + u.Surname).Contains(searchString)).ToListAsync();
            }
            else 
            {
                User = await _context.User.Where(u => u.RoleId == Constants.RoleUser).ToListAsync();
            }
        }
    }
}
