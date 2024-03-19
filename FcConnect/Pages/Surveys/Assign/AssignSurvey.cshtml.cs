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

namespace FcConnect.Pages.Surveys.Assign
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public EditModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        public List<Survey> UserCurrentSurveys { get; set; }
        public List<Survey> Surveys { get; set; }
        public SurveyUserLink surveyToAssign { get; set; }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.User.Include(u => u.Surveys).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;

            // Fetch surveys to build dropdown
            Surveys = _context.Survey.Include(s => s.Questions).ToList();

            // Load the user's current surveys
            UserCurrentSurveys = new List<Survey>();
            foreach (SurveyUserLink surveyLink in user.Surveys) 
            {
                if (surveyLink.StatusId == Constants.StatusSurveyOutstanding) 
                {
                    Survey survey = await _context.Survey.FindAsync(surveyLink.SurveyId);
                    UserCurrentSurveys.Add(survey);
                    Surveys.Remove(survey);
                }
            }

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

            TimeSpan time = new TimeSpan(17, 00, 0);

            int surveyAssigningId = int.Parse(Request.Form["surveyDrop"]);
            DateTime dueDate = DateTime.Parse(Request.Form["dueDatePicker"] + " " + time);


            surveyToAssign = new()
            {
                User = User,
                SurveyId = surveyAssigningId,
                DateDue = dueDate, //TODO - add a field to allow user to pick date
                StatusId = Constants.StatusSurveyOutstanding
            };

            _context.Add(surveyToAssign);
            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
