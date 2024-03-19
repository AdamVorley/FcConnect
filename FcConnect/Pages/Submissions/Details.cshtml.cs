using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions
{
    public class DetailsModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public DetailsModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public SurveyUserLink SurveyUserLink { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyuserlink = await _context.SurveyUserLink.FirstOrDefaultAsync(m => m.Id == id);
            if (surveyuserlink == null)
            {
                return NotFound();
            }
            else
            {
                SurveyUserLink = surveyuserlink;
            }
            return Page();
        }
    }
}
