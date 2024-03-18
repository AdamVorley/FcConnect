﻿namespace FcConnect.Models
{
    public class SurveySubmission
    {
        public int Id { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public int SubmissionSurveyLinkId { get; set; }
        public required User User { get; set; }

    }
}
