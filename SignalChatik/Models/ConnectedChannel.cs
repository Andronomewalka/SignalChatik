using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Models
{
    public class ConnectedChannel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime LastInteractionTime { get; set; }

        [Required]
        public bool IsConnectedBack { get; set; }


        [Required]
        [ForeignKey("ForChannelId")]
        public Channel For { get; set; }

        [Required]
        public int ForChannelId { get; set; }

        [Required]
        [ForeignKey("ConnectedChannelId")]
        public Channel Connected { get; set; }

        [Required]
        public int ConnectedChannelId { get; set; }
    }
}
