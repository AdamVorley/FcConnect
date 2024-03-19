using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions.Manage
{
    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public IndexModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<SurveySubmission> SurveySubmission { get; set; } = default!;
        public string CurrentFilter { get; set; }


        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                SurveySubmission = await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).OrderByDescending(s => s.SubmittedDateTime)
                .Where(s => s.User.Surname.Contains(searchString) || s.User.Forename.Contains(searchString) ||
                (s.User.Forename + " " + s.User.Surname).Contains(searchString)).ToListAsync();
            }
            else
            {

                SurveySubmission = await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).OrderByDescending(s => s.SubmittedDateTime).ToListAsync();
            }

        }
    }
}
