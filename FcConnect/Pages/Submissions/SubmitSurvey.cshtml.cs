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
using FcConnect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Submissions
{
    [Authorize(Roles = "User")]
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogEvent _logEvent;
        private readonly UserManager<IdentityUser> _userManager;

        public List<SurveyAnswer> surveyAnswers;
        public SurveySubmission surveySubmission;
        public Survey survey;
        public SurveyUserLink newSurveyUserLink;

        public EditModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, LogEvent logEvent, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logEvent = logEvent;
            _userManager = userManager;
            
        }

        [BindProperty]
        public SurveyUserLink SurveyUserLink { get; set; } = default!;
        public List<SurveyQuestion> SurveyQuestions { get; set; }
        public string SvgContent { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signedInUser = await _userManager.GetUserAsync(User);
            string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var surveyuserlink =  await _context.SurveyUserLink.Include(s => s.User).FirstOrDefaultAsync(m => m.Id == id);
            if (surveyuserlink == null)
            {
                return NotFound();
            }

            if (surveyuserlink.User.Id != signedInUser.Id) 
            {
                await _logEvent.Log("Unauthorised survey access attempt", "User " + signedInUser.Id +
                    " attempted to access a survey assigned to user " + surveyuserlink.User.Id, -1, signedInUser.Id, userIpAddress);
            }

            SurveyUserLink = surveyuserlink;

            SurveyQuestions = _context.SurveyQuestion.Where(s => s.Survey.Id == SurveyUserLink.SurveyId).ToList();

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "submit_response.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            SurveyUserLink = await _context.SurveyUserLink.Include(s => s.User).FirstOrDefaultAsync(m => m.Id == id);
            SurveyQuestions = _context.SurveyQuestion.Where(s => s.Survey.Id == SurveyUserLink.SurveyId).ToList();
            survey = await _context.Survey.FindAsync(SurveyUserLink.SurveyId);

            surveyAnswers = new List<SurveyAnswer>();

            // get number of questions
            int answerCount = SurveyQuestions.Count;

            // loop through answers and create new SurveyAnswer object for each and add to SurveyAnswers list
            for (int i = 0; i < answerCount; i++) 
            {
                string answerText = Request.Form["answerText" + i];
                if (answerText.Length > Constants.TextFieldCharLimit) 
                {

                    var signedInUser = await _userManager.GetUserAsync(User);
                    string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                    await _logEvent.Log("Char limit exceeded", "User attempted to submit data above the char limit.", -1, signedInUser.Id, userIpAddress);

                }
                if (answerText != null) 
                {
                    SurveyAnswer surveyAnswer = new()
                    {
                        Survey = survey,
                        QuestionId = i + 1,
                        AnswerText = answerText
                    };
                    surveyAnswers.Add(surveyAnswer);
                }
            }

            // Create the submissions
            surveySubmission = new()
            {
                SubmittedDateTime = GetDateTime.GetGMT(),
                User = SurveyUserLink.User,
                Survey = survey,
                Answers = surveyAnswers,
                StatusId = Constants.StatussSubmissionPendingReview
            };

            _context.SurveySubmission.Add(surveySubmission);

            // update the Survey Status to completed
            if (SurveyUserLink != null)
            {
                SurveyUserLink.StatusId = Constants.StatusSurveyCompleted;
            }

            if (SurveyUserLink.EndDate.Date > DateTime.Now) 
            {
                // Create new SurveyUserLink
                newSurveyUserLink = new()
                {
                    SurveyId = SurveyUserLink.SurveyId,
                    DateDue = SurveyUserLink.DateDue.AddDays(SurveyUserLink.SurveyFrequency),
                    EndDate = SurveyUserLink.EndDate,
                    StatusId = Constants.StatusSurveyOutstanding,
                    SurveyFrequency = SurveyUserLink.SurveyFrequency,
                    User = SurveyUserLink.User
                };

                _context.SurveyUserLink.Add(newSurveyUserLink);
            }

            _context.Attach(SurveyUserLink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyUserLinkExists(SurveyUserLink.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("./MySurveys");
        }

        private bool SurveyUserLinkExists(int id)
        {
            return _context.SurveyUserLink.Any(e => e.Id == id);
        }
    }
}
