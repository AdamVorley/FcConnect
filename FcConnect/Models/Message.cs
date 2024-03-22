using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FcConnect.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        [ValidateNever]
        public required User Sender { get; set; }
        [ValidateNever]
        public required User Recipient { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public required string MessageContent { get; set; }
        public DateTime DateTimeSent { get; set; }
        public bool IsRead {  get; set; }
    }
}
