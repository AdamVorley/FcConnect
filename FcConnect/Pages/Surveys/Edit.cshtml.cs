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

namespace FcConnect.Pages.Surveys
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public EditModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;

        }
            
        [BindProperty]
        public Survey Survey { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey =  await _context.Survey.Include(s => s.Questions).FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }
            Survey = survey;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            //tryUpdate

            // get the original Survey instance again
            Survey survey = await _context.Survey.Include(s => s.Questions).FirstOrDefaultAsync(m => m.Id == id);

            // loop through the questions entered on the page
            // Survey -> new text entered on page
            // survey -> original for comparison
            foreach (SurveyQuestion sq in Survey.Questions) 
            {   
                // fetch the existing question by the Id
                SurveyQuestion existing = await _context.SurveyQuestion.FindAsync(sq.Id);

                // check that the question is not null, and that the Id is included in the original Questions list (in case Id has been manipulated by user)
                if (existing != null && survey.Questions.Contains(existing)) 
                {
                    existing.QuestionText = sq.QuestionText;
                }
            }

        /*    for (int i = 0; i < Survey.Questions.Count; i++) 
            {
                SurveyQuestion existing = await _context.SurveyQuestion.FindAsync(survey.Questions.ElementAt(i));
                if (existing != null && survey.Questions.Contains(existing)) 
                {
                    existing.QuestionText = Survey.Questions.ElementAt(i).QuestionText;
                }

            }*/

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(Survey.Id))
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

        private bool SurveyExists(int id)
        {
            return _context.Survey.Any(e => e.Id == id);
        }
    }
}
