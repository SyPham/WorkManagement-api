using Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
   public class Role : IEntity
    {
        public int ID{ get; set; }
        public string Name { get; set; }
    }
}
