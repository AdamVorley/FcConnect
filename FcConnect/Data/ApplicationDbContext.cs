using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FcConnect.Models;

namespace FcConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FcConnect.Models.Survey> Survey { get; set; } = default!;
        public DbSet<FcConnect.Models.SurveySubmission> SurveySubmission { get; set; } = default!;
        public DbSet<FcConnect.Models.SurveyQuestion> SurveyQuestion { get; set;} = default!;
        public DbSet<FcConnect.Models.SurveyAnswer> SurveyAnswer { get; set;} = default!;
        public DbSet<FcConnect.Models.SurveyAnswer> SurveyUserLink { get; set; } = default!;


    }
}
