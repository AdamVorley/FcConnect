﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        public List<SurveyAnswer> surveyAnswers;
        public SurveySubmission surveySubmission;
        public Survey survey;
        public SurveyUserLink newSurveyUserLink;

        public EditModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SurveyUserLink SurveyUserLink { get; set; } = default!;
        public List<SurveyQuestion> SurveyQuestions { get; set; } 


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink =  await _context.SurveyUserLink.Include(s => s.User).FirstOrDefaultAsync(m => m.Id == id);
            if (surveyuserlink == null)
            {
                return NotFound();
            }
            SurveyUserLink = surveyuserlink;

            SurveyQuestions = _context.SurveyQuestion.Where(s => s.Survey.Id == SurveyUserLink.SurveyId).ToList();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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
                SubmittedDateTime = DateTime.Now,
                User = SurveyUserLink.User,
                Survey = survey,
                Answers = surveyAnswers
            };

            _context.SurveySubmission.Add(surveySubmission);

            // update the Survey Status to completed
            if (SurveyUserLink != null)
            {
                SurveyUserLink.StatusId = Constants.StatusSurveyCompleted;
            }

            // Create new SurveyUserLink
            newSurveyUserLink = new()
            {
                SurveyId = SurveyUserLink.SurveyId,
                DateDue = SurveyUserLink.DateDue.AddDays(Constants.SurveyFrequencyDays),
                StatusId = Constants.StatusSurveyOutstanding,
                User = SurveyUserLink.User                                
            };

            _context.SurveyUserLink.Add(newSurveyUserLink);


            _context.Attach(SurveyUserLink).State = EntityState.Modified;

       /*     if (!ModelState.IsValid)
            {
                return Page();
            }*/

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
