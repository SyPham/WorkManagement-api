﻿using Data.Models;
using Data.ViewModel.OC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IOCService
    {
        Task<List<TreeView>> GetListTree();
        Task<bool> AddOrUpdate(OC entity);
        Task<bool> IsExistsCode(int ID);
        Task<bool> Rename(TreeViewRename level);
        string GetNode(string code);
        string GetNode(int id);
    }
}
