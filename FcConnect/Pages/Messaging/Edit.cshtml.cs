using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.Pkcs;
using System.ComponentModel.DataAnnotations;
using FcConnect.Utilities;

namespace FcConnect.Pages.Messaging
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogEvent _logEvent;

        public EditModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, LogEvent logEvent)
        {
            _context = context;
            _userManager = userManager;
            _logEvent = logEvent;
        }

        [BindProperty]
        public Conversation Conversation { get; set; } = default!;
        public string userId;
        [BindProperty]
        [Required(ErrorMessage = "Please enter a message")]
        public string NewMessageText { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, string? user)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetString("ConversationId", id.ToString());

            var identityUser = await _userManager.GetUserAsync(User);
            var signedInUser = _context.User.Find(identityUser.Id);
            string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();


            userId = user;

            var conversation =  await _context.Conversation.Include(c => c.Users).Include(c => c.Messages).FirstOrDefaultAsync(m => m.Id == id);

            if (!conversation.Users.Contains(signedInUser)) 
            {
                // if user is trying to access a conversation they are not in
                await _logEvent.Log("Unauthorised message access attempt", "User " + signedInUser.Id + " attempted to access conversation Id " + conversation.Id + ".", -1, signedInUser.Id, userIpAddress);
                return new StatusCodeResult(403);
            }

            // mark message as read when opened
            for ( int i = conversation.Messages.Count - 1; i >= 0; i--) 
            {
                if (conversation.Messages.ElementAt(i).Sender != signedInUser && conversation.Messages.ElementAt(i).IsRead == true)
                {
                    break;
                }
                if (conversation.Messages.ElementAt(i).Sender != signedInUser && conversation.Messages.ElementAt(i).IsRead == false) 
                {
                    conversation.Messages.ElementAt(i).IsRead = true;
                }
            }

            _context.SaveChanges();

            if (conversation == null)
            {
                return NotFound();
            }
            Conversation = conversation;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var identityUser = await _userManager.GetUserAsync(User);
            var signedInUser = _context.User.Find(identityUser.Id);
            string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (Conversation.Id.ToString() != HttpContext.Session.GetString("ConversationId"))
            {
                await _logEvent.Log("Unauthorised message send attempt", 
                    "User " + signedInUser.Id + " attempted to access conversation Id " + Conversation.Id + " through url injection.", -1, signedInUser.Id, userIpAddress);

                return new StatusCodeResult(400);
            }

            var conversation = await _context.Conversation.Include(c => c.Messages).Include(c => c.Users).FirstOrDefaultAsync(c => c.Id == Conversation.Id);

            if (conversation == null) 
            {
                return new StatusCodeResult(500); 
            }

            User sender = await _context.User.FirstOrDefaultAsync(u => u.Id == identityUser.Id);
            User recipient = null;

            foreach (var user in conversation.Users) 
            {
                if (user != sender) 
                {
                    recipient = user;
                }
            }

            // update conversation
            conversation.LastMessageSent = DateTime.Now;

            // new Message - add to conversation
            Message newMessage = new()
            {
                MessageContent = NewMessageText,
                Sender = sender,
                Recipient = recipient,
                DateTimeSent = GetDateTime.GetGMT(),  
                IsRead = false
            };

            conversation.Messages.Add(newMessage);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConversationExists(Conversation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // return to refresh page and render message
            return new JsonResult(new { success = true });

        }

        private bool ConversationExists(Guid id)
        {
            return _context.Conversation.Any(e => e.Id == id);
        }
    }
}
