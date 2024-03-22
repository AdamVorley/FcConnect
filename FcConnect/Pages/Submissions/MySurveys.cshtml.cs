using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Identity;

namespace FcConnect.Pages.Submissions
{
    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<SurveyUserLink> SurveyUserLink { get;set; } = default!;
        public List<string> SurveyNames { get; set; } = default!;
        public string SvgContent { get; private set; }


        public async Task OnGetAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var user = await _context.User.FindAsync(identityUser.Id);

            SurveyUserLink = await _context.SurveyUserLink.Where(s => s.User == user && s.StatusId == Constants.StatusSurveyOutstanding).ToListAsync();

            SurveyNames = new List<string>();

            foreach (var s in SurveyUserLink) 
            {
                Survey survey = _context.Survey.Find(s.SurveyId);
                if (survey != null) 
                {
                    SurveyNames.Add(survey.Name);
                }
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "caught-up.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
