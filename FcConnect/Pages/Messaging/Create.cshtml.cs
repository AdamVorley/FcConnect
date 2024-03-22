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

namespace FcConnect.Pages.Messaging
{
    public class CreateModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // get users for recipient list
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _context.User.Find(identityUser.Id);

            UserTo = _context.User.ToList();
            UserTo.Remove(user);

            return Page();
        }

        [BindProperty]
        public Message Message { get; set; } = default!;
        public List<User> UserTo { get; set; }

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

            Message.Sender = sender;
            Message.Recipient = recipient;
            Message.DateTimeSent = DateTime.Now;
            Message.IsRead = false;            

            _context.Message.Add(Message);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
