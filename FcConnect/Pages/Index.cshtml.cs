using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FcConnect.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _userManager = userManager;
        }

        public string SvgContent { get; private set; }
        public string SvgNewSubmissions { get; private set; }

        public string SvgNewMessages { get; private set; }

        public string SvgOverdueSubmissions { get; private set; }

        public string SvgSubissionsReviewed { get; private set; }
        public string SvgSurveys {  get; private set; }
        public string UserName { get; set; }
        public int NewSubmissions { get; set; }
        public int NewMessages { get; set; }
        public int OverdueSubmissions { get; set; }
        public int SubmissionsReviewed { get; set; }
        public int Surveys { get; set; }
        public List<Message> UnreadMessages { get; set; }
        public List<SurveySubmission> Submissions { get; set; }
        public List<SurveyUserLink> Overdue {  get; set; }
        public List<SurveySubmission> Reviewed { get; set; }

        public List<SurveyUserLink> OutstandingSurveys { get; set; }  

        public async Task GetAdminData() 
        {
            var signedInUser = await _userManager.GetUserAsync(User);
            var user = await _context.User.FindAsync(signedInUser.Id);
            UserName = user.Forename;

            UnreadMessages = await _context.Message.Where(m => m.Recipient.Id == signedInUser.Id).Where(m => m.IsRead == false).ToListAsync();
            Submissions = await _context.SurveySubmission.Where(s => s.StatusId == Constants.StatussSubmissionPendingReview).ToListAsync();
            Overdue = await _context.SurveyUserLink.Where(s => s.DateDue < DateTime.Now.Date && s.StatusId == Constants.StatusSurveyOutstanding).ToListAsync();
            Reviewed = await _context.SurveySubmission.Where(s => s.ReviewedDateTime.Date == DateTime.Now.Date && s.ReviewedByUserId == signedInUser.Id).ToListAsync();

            NewMessages = UnreadMessages.Count;
            NewSubmissions = Submissions.Count;
            OverdueSubmissions = Overdue.Count;
            SubmissionsReviewed = Reviewed.Count;
        }

        public async Task GetUserData()
        {
            var signedInUser = await _userManager.GetUserAsync(User);
            var user = await _context.User.FindAsync(signedInUser.Id);
            UserName = user.Forename;

            UnreadMessages = await _context.Message.Where(m => m.Recipient.Id == signedInUser.Id).Where(m => m.IsRead == false).ToListAsync();
            Overdue = await _context.SurveyUserLink.Where(s => s.DateDue < DateTime.Now.Date && s.StatusId == Constants.StatusSurveyOutstanding && s.User == user).ToListAsync();
            OutstandingSurveys = await _context.SurveyUserLink.Where(s => s.StatusId == Constants.StatusSurveyOutstanding && s.User == user).ToListAsync();

            NewMessages = UnreadMessages.Count;
            OverdueSubmissions = Overdue.Count;
            Surveys = OutstandingSurveys.Count;

            var svgSurveyFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "surveys.svg");
            SvgSurveys = System.IO.File.ReadAllText(svgSurveyFilePath);

            var svgNewMessagesFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_message_home.svg");
            SvgNewMessages = System.IO.File.ReadAllText(svgNewMessagesFilePath);

            var svgOverdueSubmissionsFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "overdue.svg");
            SvgOverdueSubmissions = System.IO.File.ReadAllText(svgOverdueSubmissionsFilePath);
        }

        public async Task OnGet()
        {
            if (User.IsInRole("Admin")) 
            {
                await GetAdminData();
            }
            if (User.IsInRole("User"))
            {
                await GetUserData();
            }

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "welcome.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            var svgNewSubmissionsFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_submissions.svg");
            SvgNewSubmissions = System.IO.File.ReadAllText(svgNewSubmissionsFilePath);

            var svgNewMessagesFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_message_home.svg");
            SvgNewMessages = System.IO.File.ReadAllText(svgNewMessagesFilePath);

            var svgOverdueSubmissionsFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "overdue.svg");
            SvgOverdueSubmissions = System.IO.File.ReadAllText(svgOverdueSubmissionsFilePath);

            var svgSubissionsReviewedFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "reviewed_home.svg");
            SvgSubissionsReviewed = System.IO.File.ReadAllText(svgSubissionsReviewedFilePath);

        }
    }
}
