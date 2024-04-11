using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Survey Survey { get; set; } = default!;
        public bool IsEditable { get; set; }
        public string SvgContent { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? click)
        {
            if (id == null)
            {
                return NotFound();
            }

            // check user has accessed page via button click
            string clickGuid = HttpContext.Session.GetString("EditSurveyClick");

            if (click != clickGuid)
            {
                // TODO - Log
                return new StatusCodeResult(403);
            }

            HttpContext.Session.Remove("EditSurveyClick");
            var survey = await _context.Survey.FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null)
            {
                return NotFound();
            }
            else
            {
                Survey = survey;
            }

            // check if this survey has been assigned to any users
            var assignedSurveys = await _context.SurveyUserLink.Where(s => s.SurveyId == id).ToListAsync();
            IsEditable = assignedSurveys.Count == 0;

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "delete.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Survey.FindAsync(id);
            if (survey != null)
            {
                Survey = survey;
                _context.Survey.Remove(Survey);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
