namespace FcConnect.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // use navigation properties for questions:
        //   public ICollection<SurveyQuestion> Questions { get; set; }

    }
}
