using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions.Manage
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public EditModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SurveySubmission SurveySubmission { get; set; } = default!;
        public List<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();

        public async Task<IActionResult> OnGetAsync(int? id, string? click)
        {
            if (id == null)
            {
                return NotFound();
            }

            string clickGuid = HttpContext.Session.GetString("EditClick");

            if (click != clickGuid) 
            {
                return RedirectToPage("/Index");
            }

            HttpContext.Session.Remove("EditClick");

            var surveysubmission =  await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).Include(s => s.Answers).FirstOrDefaultAsync(m => m.Id == id);
            if (surveysubmission == null)
            {
                return NotFound();
            }
            SurveySubmission = surveysubmission;

            SurveyQuestions = await _context.SurveyQuestion.Where(s => s.Survey == surveysubmission.Survey).OrderBy(s => s.QuestionId).ToListAsync();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(SurveySubmission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveySubmissionExists(SurveySubmission.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SurveySubmissionExists(int id)
        {
            return _context.SurveySubmission.Any(e => e.Id == id);
        }
    }
}
