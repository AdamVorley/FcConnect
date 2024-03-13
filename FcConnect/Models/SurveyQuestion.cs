namespace FcConnect.Models
{
    public class SurveyQuestion
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public required Survey Survey { get; set; }

    }
}
