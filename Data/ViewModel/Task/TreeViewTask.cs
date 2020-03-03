using Data.Models;
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
        public string Priority { get; set; }
        public string PriorityID { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int ParentID { get; set; }
        public string DueDate { get; set; }
        public string Deadline { get; set; }
        public string CreatedDate { get; set; }
        public string Remark { get; set; }
        public string PIC { get; set; }
        public string From { get; set; }
        public string DeputyName { get; set; }
        public string ProjectName { get; set; }
        public string EveryDay { get; set; }
        public string Quarterly { get; set; }
        public string Monthly { get; set; }
        public bool Follow { get; set; }
        public int JobTypeID { get; set; }

        public int CreatedBy { get; set; }
        public int ProjectID { get; set; }
        public Data.Models.User User { get; set; }
        public List<BeAssigned> BeAssigneds { get; set; }
        public List<BeAssigned> DeputiesList { get; set; }
        public List<int> Deputies { get; set; }
        public List<int> PICs { get; set; }
        public BeAssigned FromWho { get; set; }
        public FromWhere FromWhere { get; set; }

        public Data.Models.Project Project { get; set; }

        public string state { get; set; }
        public bool FinishTask { get; set; }
    
        public bool HasChildren
        {
            get { return children.Any(); }
        }

        public List<TreeViewTask> children { get; set; }
    }
   
    public class BeAssigned
    {
        public int ID { get; set; }
        public string Username { get; set; }
    }
    public class FromWhere
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
