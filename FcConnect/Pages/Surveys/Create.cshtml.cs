using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Surveys
{
    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public CreateModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Survey Survey { get; set; } = default!;  

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }            

            _context.Survey.Add(Survey);
            await _context.SaveChangesAsync(); 

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
                    SurveyQuestion surveyQuestion = new SurveyQuestion(i + 1, Survey.Id, question);
                    _context.SurveyQuestion.Add(surveyQuestion);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
