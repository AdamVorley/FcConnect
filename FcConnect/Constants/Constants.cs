using System.Security.Policy;

public class Constants
{
    public const int StatusSurveyOutstanding = 10001;
    public const int StatusSurveyInProgress = 10002;
    public const int StatusSurveyCompleted = 10003;

    public const int StatussSubmissionPendingReview = 10010;
    public const int StatussSubmissionReviewed = 100011;

    public const int StatusUserActive = 10020;
    public const int StatusUserSuspended = 10021;
    public const int StatusUserDeleted = 10022;


    public const int RoleUser = 20001;
    public const int RoleAdmin = 20002;
    public const int RoleDeveloper = 20003;

    public const int SurveyFrequencyDays = 14;

    public const int SurveyMaxQuestions = 50;
}
