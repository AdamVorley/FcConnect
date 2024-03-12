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

namespace FcConnect.Pages.Surveys
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public EditModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Survey Survey { get; set; } = default!;
        public List<SurveyQuestion> SurveyQuestions { get; set; }
        private List<int> originalIds;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SurveyQuestions = await _context.SurveyQuestion.Where(s => s.SurveyId == id).ToListAsync();
            originalIds = SurveyQuestions.Select(q => q.Id).ToList();


            var survey =  await _context.Survey.FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            
            Survey = survey;
           
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(List<SurveyQuestion> questions)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Survey).State = EntityState.Modified;

            try
            {
                int i = 0;
                foreach (SurveyQuestion question in questions)
                {

                   /* if (question.Id != originalIds[i]) 
                    {
                        return BadRequest("Invalid operation");
                    }*/

                    SurveyQuestion existing = await _context.SurveyQuestion.FindAsync(question.Id);
                    if (existing != null)
                    {
                        existing.QuestionText = question.QuestionText;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(Survey.Id))
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

        private bool SurveyExists(int id)
        {
            return _context.Survey.Any(e => e.Id == id);
        }
    }
}
