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

    public class RemoveSurveysModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RemoveSurveysModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<SurveyUserLink> SurveyUserLink { get; set; } = default!;
        public List<Survey> Surveys { get; set; } = default!;
        public User User { get; set; }
        public string SvgContent { get; private set;}

        public async Task OnGetAsync(string? id)
        {
            SurveyUserLink = await _context.SurveyUserLink.Where(s => s.User.Id == id).ToListAsync();

            User = await _context.User.FindAsync(id);

            Surveys = new List<Survey>();

            foreach (SurveyUserLink link in SurveyUserLink) 
            {
                var survey = await _context.Survey.FirstOrDefaultAsync(s => s.Id == link.SurveyId);
                Surveys.Add(survey);
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "remove_surveys.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
