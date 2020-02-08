using Data.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class Task : IEntity
    {
        public Task()
        {
         
            CreatedDate = DateTime.Now;
        }

        public int ID { get; set; }
        public string JobName { get; set; }

        public string Description { get; set; }
        public string From { get; set; }
        public string Remark { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public bool Seen { get; set; }
        public int ProjectID { get; set; }

        [ForeignKey("User")]
        public int CreatedBy { get; set; }
        public virtual User User { get; set; }
        public int OCID { get; set; }


    }
}
