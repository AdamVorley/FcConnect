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

namespace FcConnect.Pages.Submissions.Manage
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;


        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;

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

            // check user has accessed page via button click
            string clickGuid = HttpContext.Session.GetString("EditClick");

            if (click != clickGuid) 
            {
                return new StatusCodeResult(403);
            }

            HttpContext.Session.Remove("EditClick");

            var surveysubmission =  await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).Include(s => s.Answers).FirstOrDefaultAsync(m => m.Id == id);
            if (surveysubmission == null)
            {
                return NotFound();
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(SurveySubmission surveySubmission)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            _context.SurveySubmission.Attach(surveySubmission);
            _context.Entry(surveySubmission).Property(s => s.StatusId).IsModified = true;
            surveySubmission.ReviewedDateTime = DateTime.Now;
            surveySubmission.ReviewedByUserId = identityUser.Id;

            // log the review
            Log submissionReviewedLog = new()
            {
                Name = "Submission Reviewed",
                Description = "Submission Id: " + surveySubmission.Id + " was reviewed by " + surveySubmission.ReviewedByUserId,
                Type = -1,
                IpAddress = "",
                SignedInUserId = identityUser.Id,
                TimeStamp = DateTime.Now
            };

            await _context.Log.AddAsync(submissionReviewedLog);

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
