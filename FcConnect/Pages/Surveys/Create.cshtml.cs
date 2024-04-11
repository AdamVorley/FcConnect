using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Authorization;
using FcConnect.Utilities;
using Microsoft.AspNetCore.Identity;

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogEvent _logEvent;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, LogEvent logEvent, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logEvent = logEvent;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_survey.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        [BindProperty]
        public Survey Survey { get; set; } = default!;
        public string SvgContent { get; private set; }
      
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // get the number of questions
            var questionCount = Request.Form["hiddenQuestionFieldCount"];
            int getNumFields = int.Parse(questionCount);

            // Check the number of questions retrieved from the JS is within the limit to prevent potential tampering
            if (getNumFields > Constants.SurveyMaxQuestions)
            {
                string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                var identityUser = await _userManager.GetUserAsync(User);
                string userId = "Unknown";

                if (identityUser != null)
                {
                    userId = identityUser.Id;
                }

                await _logEvent.Log("Tamper Alert - Create Survey", "The getNumFields value was above the specified limit. Likely result of malicious input by user", -1, userId, userIpAddress);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            }

            for (int i = 0; i < getNumFields; i++)
            {
                string question = Request.Form["QuestionText" + i];

                if (question.Length > Constants.TextFieldCharLimit) 
                {
                    // if text length is longer than permitted, user has gone out of their way to enter too much text
                    string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    var identityUser = await _userManager.GetUserAsync(User);
                    string userId = "Unknown";

                    if (identityUser != null) 
                    {
                        userId = identityUser.Id;
                    }

                    await _logEvent.Log("Tamper Alert - Create Survey", "The character limit was exceeded. Likely result of malicious input by user", -1, userId, userIpAddress);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                if (question != null)
                {


                    SurveyQuestion surveyQuestion = new()
                    {
                        QuestionId = i + 1,
                        QuestionText = question,
                        Survey = Survey
                    };
                    Survey.Questions.Add(surveyQuestion);
                }
            }

            _context.Survey.Add(Survey);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
