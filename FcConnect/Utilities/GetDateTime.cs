namespace FcConnect.Utilities
{
    public class GetDateTime
    {
        static DateTime timeUtc = DateTime.UtcNow;
        static TimeZoneInfo gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public static DateTime GetGMT() 
        {
            DateTime gmtDateTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, gmtZone);
            return gmtDateTime;
        }
    }
}
