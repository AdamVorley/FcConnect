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

namespace FcConnect.Pages.Messaging
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

        public IList<Message> Message { get;set; } = default!;
        public IList<Conversation> Conversation { get; set; }
        public string userId;
        public string SvgContent { get; private set; }
        public string SvgHeaderContent { get; private set; }
        public string CurrentFilter { get; set; }


        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            var identityUser = await _userManager.GetUserAsync(User);
            var user = _context.User.Find(identityUser.Id);

            userId = user.Id.ToString();

            if (!String.IsNullOrEmpty(searchString))
            {

                Conversation = await _context.Conversation.Where(c => c.Users.Contains(user) && c.Messages.Any(m => m.Recipient == user) 
                && c.Messages.Any(m => m.Sender.Forename.Contains(searchString) || m.Sender.Surname.Contains(searchString) || 
                (m.Sender.Forename + " " + m.Sender.Surname).Contains(searchString))).OrderByDescending(c => c.LastMessageSent)
                .Include(c => c.Messages).Include(c => c.Users).ToListAsync();

            }
            else 
            {
                Conversation = await _context.Conversation.Where(c => c.Users.Contains(user) && c.Messages.Any(m => m.Recipient == user)).OrderByDescending(c => c.LastMessageSent)
                .Include(c => c.Messages).Include(c => c.Users).ToListAsync();
            }


            var svgHeaderFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "inbox.svg");
            SvgHeaderContent = System.IO.File.ReadAllText(svgHeaderFilePath);

            if (Conversation.Count < 1) 
            {            
                var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "no_messages.svg");
                SvgContent = System.IO.File.ReadAllText(svgFilePath);
            }

            Message = await _context.Message.Where(m => m.Recipient == user).OrderByDescending(m => m.DateTimeSent).ToListAsync();
        }
    }
}
