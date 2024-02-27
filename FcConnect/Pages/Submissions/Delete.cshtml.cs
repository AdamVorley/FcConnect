using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions
{
    public class DeleteModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public DeleteModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SurveySubmission SurveySubmission { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveysubmission = await _context.SurveySubmission.FirstOrDefaultAsync(m => m.Id == id);

            if (surveysubmission == null)
            {
                return NotFound();
            }
            else
            {
                SurveySubmission = surveysubmission;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveysubmission = await _context.SurveySubmission.FindAsync(id);
            if (surveysubmission != null)
            {
                SurveySubmission = surveysubmission;
                _context.SurveySubmission.Remove(SurveySubmission);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
