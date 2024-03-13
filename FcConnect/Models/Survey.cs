using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FcConnect.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [ValidateNever]
        public virtual ICollection<SurveyQuestion>? Questions { get; set; } = new List<SurveyQuestion>();

    }
}