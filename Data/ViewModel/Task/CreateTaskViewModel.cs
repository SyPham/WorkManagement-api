﻿using Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModel.Task
{
   public class CreateTaskViewModel
    {
        public CreateTaskViewModel()
        {
            //if (this.Remark == "" || this.Remark == null)
            //    this.Remark = "#N/A";

            //if (this.Description == "" || this.Description == null)
            //    this.Description = "#N/A";
            //if (this.JobName == "" || this.JobName == null)
            //    this.JobName = "#N/A";
        }

        public int ID { get; set; }
        public string From { get; set; }
        public string JobName { get; set; }
        public int CreatedBy { get; set; }
        public int ProjectID { get; set; }
        public int ParentID { get; set; }
        public int OCID { get; set; }
        public string Priority { get; set; }
        public bool Status { get; set; }
        public int FromWhoID { get; set; }
        public int Level { get; set; }
        public int DepartmentID { get; set; }
        public int[] PIC { get; set; }
        public int[] Deputies { get; set; }

        public int CurrentUser { get; set; }
        public int UserID { get; set; }
        public JobType JobTypeID { get; set; }
        public PeriodType periodType { get; set; }
        public string DueDateDaily { get; set; }
        public string DueDateWeekly { get; set; }
        public string DueDateMonthly { get; set; }
        public string DueDateQuarterly { get; set; }
        public string DueDateYearly { get; set; }
        public string SpecificDate { get; set; }
        public string DateOfWeekly { get; set; }

    }
}
