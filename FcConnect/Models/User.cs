﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FcConnect.Models
{
    public class User
    {
        public required string Id { get; set; }
        [ValidateNever]
        public required string Forename { get; set; }
        [ValidateNever]
        public required string Surname { get; set; }
        public ICollection<SurveyUserLink> Surveys { get; set; } = new List<SurveyUserLink>();
        public ICollection<SurveySubmission> Submissions { get; set; } = new List<SurveySubmission>();
    }
}
