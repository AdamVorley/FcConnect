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
using Microsoft.AspNetCore.Identity;
using FcConnect.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Submissions.Manage
{
    [Authorize(Roles = "Admin")]

    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogEvent _logEvent;

        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, LogEvent logEvent)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logEvent = logEvent;
        }

        [BindProperty]
        public SurveySubmission SurveySubmission { get; set; } = default!;
        public List<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();
        public string SvgContent { get; private set; }
        public string ReviewedByName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? click)
        {
            if (id == null)
            {
                return NotFound();
            }
            var identityUser = await _userManager.GetUserAsync(User);
            string userId = "Unknown";
            if (identityUser != null) { userId = identityUser.Id; }
            string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();


            // check user has accessed page via button click
            string clickGuid = HttpContext.Session.GetString("EditClick");

            if (click != clickGuid) 
            {
                // Log that user tried to access submission via url rather than button click
                await _logEvent.Log("Unauthrorised access attempt - view submission", "Click GUID was invalid", -1, userId, userIpAddress);
                return new StatusCodeResult(403);
            }

            HttpContext.Session.Remove("EditClick");

            var surveysubmission =  await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).Include(s => s.Answers).FirstOrDefaultAsync(m => m.Id == id);

            if (surveysubmission == null)
            {
                return NotFound();
            }

            if (surveysubmission.ReviewerId != identityUser.Id) 
            {
                // user attempting to view submission is not authorised 
                await _logEvent.Log("Unauthrorised access attempt - view submission", "Signed in user Id did not match the Survey Assignee Id", -1, userId, userIpAddress);
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            SurveySubmission = surveysubmission;

            SurveyQuestions = await _context.SurveyQuestion.Where(s => s.Survey == surveysubmission.Survey).OrderBy(s => s.QuestionId).ToListAsync();

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "review_submission.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            if (surveysubmission.StatusId == Constants.StatussSubmissionReviewed)
            {
                var userReviewed = await _context.User.FindAsync(surveysubmission.ReviewedByUserId);
                ReviewedByName = userReviewed.Forename + " " + userReviewed.Surname;
            }
            else 
            {
                ReviewedByName = "";
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(SurveySubmission surveySubmission)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            _context.SurveySubmission.Attach(surveySubmission);
            _context.Entry(surveySubmission).Property(s => s.StatusId).IsModified = true;
            surveySubmission.ReviewedDateTime = GetDateTime.GetGMT();
            surveySubmission.ReviewedByUserId = identityUser.Id;

            // log the review
            await _logEvent.Log("Submission Reviewed", "Submission Id: " + surveySubmission.Id + " was reviewed by " + surveySubmission.ReviewedByUserId, -1, identityUser.Id, "");           

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
