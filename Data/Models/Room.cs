using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
  public  class Room
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public bool Type { get; set; }
    }
}
