﻿using System;
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
using Microsoft.IdentityModel.Tokens;
using FcConnect.Utilities;

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

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            // get users for recipient list
            if (!id.IsNullOrEmpty())
            {
                UserTo = new List<User>();
                var userToAdd = await _context.User.FindAsync(id);
                UserTo.Add(userToAdd);
            }
            else 
            {
                var identityUser = await _userManager.GetUserAsync(User);
                var user = await _context.User.FindAsync(identityUser.Id);

                // ensure regular users can't message other regular users
                if (user.RoleId == Constants.RoleUser)
                {
                    UserTo = await _context.User.Where(u => u.UserStatusId == Constants.StatusUserActive && u.RoleId == Constants.RoleAdmin).ToListAsync();
                }
                else 
                {
                    UserTo = await _context.User.Where(u => u.UserStatusId == Constants.StatusUserActive).ToListAsync();
                }
                UserTo.Remove(user);
            }
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "new_message.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            return Page();
        }

        [BindProperty]
        public Message Message { get; set; } = default!;
        public List<User> UserTo { get; set; }
        public string SvgContent { get; set;}


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
            Message.DateTimeSent = GetDateTime.GetGMT();
            Message.IsRead = false;

            if (ConversationExists(sender, recipient))
            {
                var conversation = await _context.Conversation.FirstOrDefaultAsync(c => c.Users.Contains(sender) && c.Users.Contains(recipient));
                conversation.LastMessageSent = GetDateTime.GetGMT();
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
                    LastMessageSent = GetDateTime.GetGMT()

                };
                _context.Conversation.Add(conversation);
            }
            
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
