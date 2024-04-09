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

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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
                return Page(); //TODO return error message and audit - possibly blacklist if above certain threshold.
            }

            for (int i = 0; i < getNumFields; i++)
            {
                string question = Request.Form["QuestionText" + i];

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
