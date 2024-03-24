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

namespace FcConnect.Pages.Messaging
{
    public class EditModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Conversation Conversation { get; set; } = default!;
        public string userId;
        [BindProperty]

        public string NewMessageText { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, string? user)
        {
            if (id == null)
            {
                return NotFound();
            }

            userId = user;

            var conversation =  await _context.Conversation.Include(c => c.Users).Include(c => c.Messages).FirstOrDefaultAsync(m => m.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }
            Conversation = conversation;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var conversation = await _context.Conversation.Include(c => c.Messages).Include(c => c.Users).FirstOrDefaultAsync(c => c.Id == Conversation.Id);
                
            var identityUser = await _userManager.GetUserAsync(User);


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
                DateTimeSent = DateTime.Now,
                IsRead = false
            };

            conversation.Messages.Add(newMessage);

           // _context.Attach(Conversation).State = EntityState.Modified;

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
            return RedirectToPage("/Edit", new { id = conversation.Id }); 

            // return RedirectToPage("./Edit");
        }

        private bool ConversationExists(Guid id)
        {
            return _context.Conversation.Any(e => e.Id == id);
        }
    }
}
