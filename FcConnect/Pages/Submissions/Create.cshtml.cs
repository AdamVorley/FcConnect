using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions
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
        public SurveyUserLink SurveyUserLink { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SurveyUserLink.Add(SurveyUserLink);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
