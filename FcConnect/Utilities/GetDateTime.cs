namespace FcConnect.Utilities
{
    public class GetDateTime
    {
        static TimeZoneInfo gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public static DateTime GetGMT() 
        {
            DateTime timeUtc = DateTime.UtcNow;
            DateTime gmtDateTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, gmtZone);
            return gmtDateTime;
        }
    }
}
