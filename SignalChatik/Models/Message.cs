using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Models
{
    public class Message
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public Channel SenderChannel { get; set; }

        [Required]
        public Channel ReceiverChannel { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
