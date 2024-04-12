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
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
            
        [BindProperty]
        public Survey Survey { get; set; } = default!;
        public string SvgContent { get; private set; }
        public bool IsEditable { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? click)
        {
            if (id == null)
            {
                return NotFound();
            }

            // check user has accessed page via button click
            string clickGuid = HttpContext.Session.GetString("EditSurveyClick");

            if (click != clickGuid)
            {
                // TODO - Log
                return new StatusCodeResult(403);
            }

            HttpContext.Session.Remove("EditSurveyClick");

            var survey =  await _context.Survey.Include(s => s.Questions).FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }
            Survey = survey;

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "update.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            // check if this survey has been assigned to any users
            var assignedSurveys = await _context.SurveyUserLink.Where(s => s.SurveyId == id).ToListAsync();
            IsEditable = assignedSurveys.Count == 0;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {

            // get the original Survey instance again
            Survey survey = await _context.Survey.Include(s => s.Questions).FirstOrDefaultAsync(m => m.Id == id);
            survey.Name = Survey.Name;

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
