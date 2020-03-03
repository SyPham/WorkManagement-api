using Data.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class Task : IEntity
    {
        public Task()
        {
         
            CreatedDate = DateTime.Now;
            EveryDay = "#N/A";
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
        public int DepartmentID { get; set; }
        public bool Seen { get; set; }
        public bool FinishedMainTask { get; set; }

        public int ProjectID { get; set; }

        [ForeignKey("User")]
        public int CreatedBy { get; set; }
        public virtual User User { get; set; }
        public int OCID { get; set; }
        public int JobTypeID { get; set; }
        public int FromWhoID { get; set; }
        [MaxLength(2)]
        public string Priority { get; set; } = "M";
        public string EveryDay { get; set; }
        public string Monthly { get; set; }
        public string Quarterly { get; set; }

    }
}
