 namespace FcConnect.Models
{
    public class SurveyUserLink
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int SurveyId { get; set; }
        public DateTime DateDue { get; set; }
        public int StatusId { get; set; }
    }
}
