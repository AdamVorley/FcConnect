using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FcConnect.Models
{
    public class SurveyUserLink : IValidatableObject
    {
        public int Id { get; set; }
        public int SurveyId { get; set; } 

        [DataType(DataType.Date)]
        public DateTime DateDue { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate {  get; set; }
        public int StatusId { get; set; }
        [Range(1, 365, ErrorMessage = "Survey frequency must be between {1} and {2}.")]
        public int SurveyFrequency { get; set; }
        [ValidateNever]
        public required User User { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= DateDue)
            {
                yield return new ValidationResult("End date must be greater than inital due date", new[] { nameof(EndDate) });
            }
        }
    }
}
