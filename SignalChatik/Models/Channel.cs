using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public abstract class Channel : Clustered
    {
        [Required]
        public ChannelContent Content { get; set; }

        //[Required]
        //public ICollection<Message> SenderMessages { get; set; }
        //public int SenderMessagesId { get; set; }

        //[Required]
        //public ICollection<Message> ReceiverMessages { get; set; }
        //public int ReceiverMessagesId { get; set; }
    }
}
