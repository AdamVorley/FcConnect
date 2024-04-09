using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions.Manage
{
    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<SurveySubmission> SurveySubmission { get; set; } = default!;
        public string CurrentFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ReviewedCheckHidden { get; set; }
        public string SvgContent { get; set; }




        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            // Retrieve the value of the checkbox
            bool showReviewedSubmissions = ReviewedCheckHidden;
            int statusSubmissionId = Constants.StatussSubmissionPendingReview;

            if (!showReviewedSubmissions)
            {
                statusSubmissionId = Constants.StatussSubmissionPendingReview;
            }
            else 
            {
                statusSubmissionId = Constants.StatussSubmissionReviewed;
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "Submissions.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            if (!String.IsNullOrEmpty(searchString))
            {
                SurveySubmission = await _context.SurveySubmission.Include(s => s.User).Include(s => s.Survey).OrderByDescending(s => s.SubmittedDateTime)
                .Where(s => s.User.Surname.Contains(searchString) || s.User.Forename.Contains(searchString) ||
                (s.User.Forename + " " + s.User.Surname).Contains(searchString)).Where(s => s.StatusId <= statusSubmissionId).ToListAsync();
            }
            else
            {

                SurveySubmission = await _context.SurveySubmission.Where(s => s.StatusId <= statusSubmissionId).Include(s => s.User).Include(s => s.Survey).OrderByDescending(s => s.SubmittedDateTime).ToListAsync();
            }

        }
    }
}
