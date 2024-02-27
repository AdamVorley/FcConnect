namespace FcConnect.Models
{
    public class SurveyAnswer
    {
        public int Id { get; set; }
        public int SubmissionLinkId { get; set; }
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string AnswerText { get; set; }

        public SurveyAnswer(int submissionLinkId, int questionId, int surveyId, string answerText)
        {            
            SubmissionLinkId = submissionLinkId;
            QuestionId = questionId;
            SurveyId = surveyId;
            AnswerText = answerText;
        }
    }
}
