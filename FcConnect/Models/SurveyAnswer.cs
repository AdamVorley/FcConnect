namespace FcConnect.Models
{
    public class SurveyAnswer
    {
        public int Id { get; set; }
        public required Survey Survey { get; set; }
        public int QuestionId { get; set; }
        public required string AnswerText { get; set; }


    }
}
