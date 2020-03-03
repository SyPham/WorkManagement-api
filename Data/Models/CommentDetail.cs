using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
   public class CommentDetail
    {
        public int ID { get; set; }
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public bool Seen { get; set; }
    }
}
