using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PodBotCSharp.Models
{
    public class UserChannelData
    {
        public string ChannelId { get; set; }
        public string InstagramToken { get; set; }
        public long InstagramId { get; set; }
        public string Name { get; set; }
        public string ProfilePictureURL { get; set; }
        public string IgHandle { get; set; }
    }
}