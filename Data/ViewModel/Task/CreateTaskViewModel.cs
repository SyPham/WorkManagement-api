using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModel.Task
{
   public class CreateTaskViewModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string JobName { get; set; }
        public string Deadline { get; set; }

        public int CreatedBy { get; set; }
        public int ProjectID { get; set; }
        public int ParentID { get; set; }
        public int OCID { get; set; }

        public string Remark { get; set; }

        public bool Status { get; set; }

        public int[] PIC { get; set; }
    }
}
