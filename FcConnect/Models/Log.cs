using System.Security.Policy;

namespace FcConnect.Models
{
    public class Log
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Type { get; set; }
        public required string IpAddress { get; set; }
        public required string SignedInUserId { get; set; }
    }
}
