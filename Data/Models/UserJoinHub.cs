using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
   public class UserJoinHub
    {
        public UserJoinHub()
        {
            this.CreatedDate = DateTime.Now.ToString("dddd MM, yyyy HH:mm:ss tt");
        }

        public int ID { get; set; }
        public string ConnectionId { get; set; }
        public string CreatedDate { get; set; }
        public string Content { get; set; }
    }
}
