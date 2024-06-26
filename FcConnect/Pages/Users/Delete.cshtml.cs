﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using SendGrid.Helpers.Mail;
using Azure.Core;
using FcConnect.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogEvent _logEvent;

        public DeleteModel(FcConnect.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, LogEvent logEvent)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logEvent = logEvent;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        public string SvgContent { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string click)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (click != HttpContext.Session.GetString("UserEditClick"))
            {
                // get signed in user info
                var signedInUserId = HttpContext.Session.GetString("SignedInUserId");
                string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                // audit
                await _logEvent.Log("Error Forbidden - Delete User", "User Id: " + signedInUserId + " attempted to access delete user page for User Id: " + id +" through improper path.", -1, signedInUserId, userIpAddress);

                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            HttpContext.Session.Remove("UserEditClick");

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "delete_user.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);

            // set user status to deleted and update details to deleted
            if (user != null)
            {
                User = user;
                User.UserStatusId = Constants.StatusUserDeleted;
                User.Forename = "Deleted";
                User.Surname = "User";
                User.Email = "deleted@user.com";
            }

            // get user's submissions and set status to deleted
            var userSurveys = await _context.SurveySubmission.Where(s => s.User == user).ToListAsync();
            foreach (var s in userSurveys)
            {
                s.StatusId = Constants.StatussSubmissionDeleted;
            }

            // get any assigned surveys and delete
            var userSurveyLinks = await _context.SurveyUserLink.Where(s => s.User == user).ToListAsync();
            foreach (var s in userSurveyLinks)
            {
                _context.SurveyUserLink.Remove(s);
            }

            // get and delete identity user
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            await _userManager.DeleteAsync(identityUser);

            // get signed in user info
            var signedInUserId = HttpContext.Session.GetString("SignedInUserId");
            string userIpAddress =  HttpContext.Connection.RemoteIpAddress.ToString();

            // audit
            await _logEvent.Log("User Deleted", "User Id: " + user.Id + " was deleted by User Id: " + signedInUserId, -1, signedInUserId, userIpAddress);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
