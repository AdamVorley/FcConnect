namespace FcConnect.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public DateTime LastMessageSent { get; set; }
    }
}
