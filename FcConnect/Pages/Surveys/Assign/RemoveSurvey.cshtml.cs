using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Surveys.Assign
{
    public class RemoveSurveyModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public RemoveSurveyModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SurveyUserLink SurveyUserLink { get; set; } = default!;
        public string SurveyName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink = await _context.SurveyUserLink.FirstOrDefaultAsync(m => m.Id == id);

            if (surveyuserlink == null)
            {
                return NotFound();
            }
            else
            {
                SurveyUserLink = surveyuserlink;
            }
            var survey = await _context.Survey.FirstOrDefaultAsync(s => s.Id == surveyuserlink.SurveyId);
            SurveyName = survey.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink = await _context.SurveyUserLink.FindAsync(id);
            if (surveyuserlink != null)
            {
                SurveyUserLink = surveyuserlink;
                _context.SurveyUserLink.Remove(SurveyUserLink);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
