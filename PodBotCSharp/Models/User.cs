using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PodBotCSharp.Models
{
    public class User
    {
        public User()
        {
        }
        public int UserId { get; set; }
        // For us to save the user's id to "hard" bind the Oauth to him
        public string TelegramId { get; set; }
        public string Username { get; set; }   
        public string Password { get; set; }
        // Instagram Object Foreign Key
        public string InstagramToken { get; set; }
    }
}