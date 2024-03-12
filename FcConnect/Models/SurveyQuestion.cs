namespace FcConnect.Models
{
    public class SurveyQuestion
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string? QuestionText { get; set; }
        // AUse navigation properties instead of SurveyId:
       // public Survey Survey { get; set; }


        /*  public SurveyQuestion(int questionId, int surveyId, string questionText)
          {            
              QuestionId = questionId;
              SurveyId = surveyId;
              QuestionText = questionText;
          }*/
    }
}
