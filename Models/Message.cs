using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasecampMVC.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid MessageThreadId { get; set; }
        [ForeignKey("MessageThreadId")]
        public MessageThread Thread { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}