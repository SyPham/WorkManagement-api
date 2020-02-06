using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.ViewModel.Task
{
    public class TreeViewTask
    {
        public TreeViewTask()
        {
            this.children = new List<TreeViewTask>();
        }

        public int ID { get; set; }
        public string JobName { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int ParentID { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Remark { get; set; }
        public string From { get; set; }
        public int ProjectID { get; set; }

        public bool state { get; set; }

        public bool HasChildren
        {
            get { return children.Any(); }
        }

        public List<TreeViewTask> children { get; set; }
    }
}
