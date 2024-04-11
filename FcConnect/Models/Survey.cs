using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FcConnect.Models
{
    public class Survey
    {
        public int Id { get; set; }
        [StringLength(250)]
        public required string Name { get; set; }
        [ValidateNever]
        public virtual ICollection<SurveyQuestion>? Questions { get; set; } = new List<SurveyQuestion>();

    }
}