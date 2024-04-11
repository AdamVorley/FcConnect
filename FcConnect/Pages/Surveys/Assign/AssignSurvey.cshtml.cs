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
using System.ComponentModel.DataAnnotations;

namespace FcConnect.Pages.Surveys.Assign
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
        public User User { get; set; } = default!;
        [BindProperty]
        public List<Survey> UserCurrentSurveys { get; set; }
        [BindProperty]
        public List<Survey> Surveys { get; set; }       
        public SurveyUserLink surveyToAssign { get; set; }
        [BindProperty]
        public SurveyUserLink SurveyUserLink { get; set; } = default!;
        public string SvgContent { get; private set; }

        public async void BuildDropdowns(string id, User user) 
        {
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

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "assign_survey.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }

        public async Task<IActionResult> OnGetAsync(string id, string? click)
        {
            if (id == null)
            {
                return NotFound();
            }

            // check user has accessed page via button click
            string clickGuid = HttpContext.Session.GetString("AssignSurveyClick");

            if (click != clickGuid)
            {
                return new StatusCodeResult(403);
            }

            HttpContext.Session.Remove("AssignSurveyClick");

            var user = await _context.User.Include(u => u.Surveys).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            User = user;

            BuildDropdowns(id, user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                // if page reload required, retrieve everything again
                var user_ = await _context.User.Include(u => u.Surveys).FirstOrDefaultAsync(m => m.Id == id);
                if (user_ == null)
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                User = user_;
                BuildDropdowns(id, user_);

                return Page();
            }

            TimeSpan time = new TimeSpan(17, 00, 0);
            int surveyAssigningId;  

            try
            {
                surveyAssigningId = int.Parse(Request.Form["surveyDrop"]);
            }
            catch 
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            var user = await _context.User.Include(u => u.Surveys).FirstOrDefaultAsync(u => u.Id == id);

            surveyToAssign = SurveyUserLink;
            surveyToAssign.User = user;
            surveyToAssign.StatusId = Constants.StatusSurveyOutstanding;
            surveyToAssign.DateDue = surveyToAssign.DateDue + time; // set time to 5pm
            surveyToAssign.EndDate = surveyToAssign.EndDate + time;
            surveyToAssign.SurveyId = surveyAssigningId;

            _context.Add(surveyToAssign);

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
