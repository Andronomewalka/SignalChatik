using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalChatik.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Data { get; set; }


        [ForeignKey("SenderId")]
        public Channel Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public Channel Receiver { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
    }
}
