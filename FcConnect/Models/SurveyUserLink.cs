using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FcConnect.Models
{
    public class SurveyUserLink
    {
        public int Id { get; set; }
        public int SurveyId { get; set; } //TODO update to Survey
        public DateTime DateDue { get; set; }
        public DateTime EndDate {  get; set; }
        public int StatusId { get; set; }
        public int SurveyFrequency { get; set; }
        public required User User { get; set; }
    }
}
