namespace FcConnect.Models
{
    public class SurveyQuestion
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string QuestionText { get; set; }

        public SurveyQuestion(int id, int questionId, int surveyId, string questionText)
        {
            Id = id;
            QuestionId = questionId;
            SurveyId = surveyId;
            QuestionText = questionText;
        }
    }
}
