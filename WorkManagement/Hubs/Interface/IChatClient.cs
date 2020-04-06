using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Hubs.Interface
{
   public interface IChatClient
    {
        Task SendMessage();
        Task ReceiveMessage();
    }
}
