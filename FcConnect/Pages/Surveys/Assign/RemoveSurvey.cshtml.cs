using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Surveys.Assign
{
    [Authorize(Roles = "Admin")]

    public class RemoveSurveyModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RemoveSurveyModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public SurveyUserLink SurveyUserLink { get; set; } = default!;
        public string SurveyName { get; set; }
        public string SvgContent { get; private set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink = await _context.SurveyUserLink.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);

            if (surveyuserlink == null)
            {
                return NotFound();
            }
            else
            {
                SurveyUserLink = surveyuserlink;
            }
            var survey = await _context.Survey.FirstOrDefaultAsync(s => s.Id == surveyuserlink.SurveyId);
            SurveyName = survey.Name;

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "remove_surveys.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink = await _context.SurveyUserLink.FindAsync(id);
            if (surveyuserlink != null)
            {
                SurveyUserLink = surveyuserlink;
                _context.SurveyUserLink.Remove(SurveyUserLink);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
