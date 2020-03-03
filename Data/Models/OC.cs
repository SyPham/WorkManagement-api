using Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class OC: IEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int ParentID { get; set; }
        public virtual List<Manager> Managers { get; set; }
        public virtual List<TeamMember> TeamMembers { get; set; }

    }
}
