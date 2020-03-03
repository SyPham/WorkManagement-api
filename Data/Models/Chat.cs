using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Chat
    {
        public Chat()
        {
            CreatedTime = DateTime.Now;
        }

        public int ID { get; set; }
        public int RoomID { get; set; }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
