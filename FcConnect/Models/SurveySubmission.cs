namespace FcConnect.Models
{
    public class SurveySubmission
    {
        public int Id { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public required User User { get; set; }
        public required Survey Survey { get; set; }
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();

    }
}
