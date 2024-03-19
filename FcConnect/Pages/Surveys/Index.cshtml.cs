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

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public IndexModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Survey> Survey { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Survey = await _context.Survey.ToListAsync();
        }
    }
}
