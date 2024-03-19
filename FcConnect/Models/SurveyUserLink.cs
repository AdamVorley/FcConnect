using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FcConnect.Models
{
    public class SurveyUserLink
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public DateTime DateDue { get; set; }
        public int StatusId { get; set; }
        //[ValidateNever]
        public required User User { get; set; }
    }
}
