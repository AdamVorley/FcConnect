using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FcConnect.Models
{
    public class User
    {
        [ValidateNever]
        public required string Id { get; set; }
        [ValidateNever]
        public required string Forename { get; set; }
        [ValidateNever]
        public required string Surname { get; set; }
        [ValidateNever]
        public required string Email { get; set; }
        public int RoleId { get; set; }
        public ICollection<SurveyUserLink> Surveys { get; set; } = new List<SurveyUserLink>();
        public ICollection<SurveySubmission> Submissions { get; set; } = new List<SurveySubmission>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
