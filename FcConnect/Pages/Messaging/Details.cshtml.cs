using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Messaging
{
    public class DetailsModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public DetailsModel(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Message> Messages { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? sender, string? receiver)
        {
            if (sender == null || receiver == null)
            {
                return NotFound();
            }

            var message = _context.Message.Where(m => m.Sender.Id == sender && m.Recipient.Id == receiver 
            || m.Sender.Id == receiver && m.Recipient.Id == sender).Include(m => m.Sender).Include(m => m.Recipient).ToList();
            if (message == null)
            {
                return NotFound();
            }
            else
            {
                Messages = message;
            }
            return Page();
        }
    }
}
