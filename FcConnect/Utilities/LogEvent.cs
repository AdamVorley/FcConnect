using FcConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FcConnect.Utilities
{
    public class LogEvent : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;

        public LogEvent(FcConnect.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Log(string name, string description, int type, string userId, string ipAddress)
        {
            if (userId.IsNullOrEmpty())
            {
                userId = "NotFound";
            }

            Log logEvent = new()
            {
                Name = name,
                Description = description,
                Type = type,
                IpAddress = ipAddress,
                SignedInUserId = userId,
                TimeStamp = GetDateTime.GetGMT()
            };

            await _context.Log.AddAsync(logEvent);
            await _context.SaveChangesAsync();
        }
    }
}
