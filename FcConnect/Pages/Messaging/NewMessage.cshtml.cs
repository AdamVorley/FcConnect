using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FcConnect.Pages.Messaging
{
    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // get users for recipient list
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _context.User.Find(identityUser.Id);

            UserTo = _context.User.ToList();
            UserTo.Remove(user);

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_message.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        [BindProperty]
        public Message Message { get; set; } = default!;
        public List<User> UserTo { get; set; }
        public string SvgContent { get; set;}


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string recipientUserId = Request.Form["userToDrop"];

            var identityUser = await _userManager.GetUserAsync(User);
            var sender = _context.User.Find(identityUser.Id);
            var recipient = _context.User.Find(recipientUserId);

            if(sender == null || recipient == null) 
            {
                return new StatusCodeResult(500);
            }

            Message.Sender = sender;
            Message.Recipient = recipient;
            Message.DateTimeSent = DateTime.Now;
            Message.IsRead = false;

            if (ConversationExists(sender, recipient))
            {
                var conversation = await _context.Conversation.FirstOrDefaultAsync(c => c.Users.Contains(sender) && c.Users.Contains(recipient));
                conversation.LastMessageSent = DateTime.Now;
                conversation.Messages.Add(Message);
            }
            else 
            {
                List<User> users = new List<User>();
                List<Message> messages = new List<Message>();

                users.Add(sender);
                users.Add(recipient);

                messages.Add(Message);

                Conversation conversation = new()
                {
                    Users = users,
                    Messages = messages,
                    LastMessageSent = DateTime.Now                 

                };
                _context.Conversation.Add(conversation);
            }
            
           // _context.Message.Add(Message);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Messages");
        }

        public bool ConversationExists(User sender, User recipient)
        {
            var conversation = _context.Conversation.Where(c => c.Users.Contains(sender) && c.Users.Contains(recipient)).ToList();
            return conversation.Count > 0;
        }

    }



}
