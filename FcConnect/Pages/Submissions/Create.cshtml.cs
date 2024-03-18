using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
// authorise this page for users
namespace FcConnect.Pages.Submissions
{
    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {

            return Page();
        }

        public void LoadSurveyQuestions() 
        {
            // fetch the questions from the db 
            var questions = _context.SurveyQuestion.ToList();

            // this needs to be generated from survey user linke. this will load links by user id ie. display all surveys for current user. upon selecting will then load by survey id.

            if (questions.Count > 0)
            {
                Console.Write(questions[0]);
            }
  
        }

        [BindProperty]
        public SurveySubmission SurveySubmission { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();

            var signedInUser = await _userManager.GetUserAsync(User);
            User user = await _context.User.FindAsync(signedInUser.Id);

            SurveySubmission.SubmittedDateTime = DateTime.Now;
            SurveySubmission.User = user; //TODO - display user name instead of id, keep Id on database.

            SurveyAnswer surveyAnswer = new SurveyAnswer(SurveySubmission.Id, 1, 1, "test"); // get answers from page

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SurveySubmission.Add(SurveySubmission);
            _context.SurveyAnswer.Add(surveyAnswer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
