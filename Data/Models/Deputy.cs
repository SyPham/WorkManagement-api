using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
   public class Deputy
    {
        [Key]
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int UserID { get; set; }
     

    }
}
