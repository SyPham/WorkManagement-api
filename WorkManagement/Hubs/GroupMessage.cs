using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Hubs
{
    public class GroupMessage
    {
        public int UserID { get; set; }
        public string Message { get; set; }
        public int Room { get; set; }
    }
}
