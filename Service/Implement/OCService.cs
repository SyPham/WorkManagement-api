using Data;
using Data.Models;
using Data.ViewModel.OC;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class OCService : IOCService
    {
        private readonly DataContext _context;

        public OCService(DataContext context)
        {
            _context = context;
        }

        public Task<bool> AddOrUpdate(OC entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TreeView>> GetListTree()
        {
            var listLevels = await _context.OCs.OrderBy(x => x.Level).ToListAsync();
            var levels = new List<TreeView>();
            foreach (var item in listLevels)
            {
                var levelItem = new TreeView();
                levelItem.key = item.ID;
                levelItem.title = item.Name;
                levelItem.levelnumber = item.Level;
                levelItem.parentid = item.ParentID;
                levels.Add(levelItem);
            }

            List<TreeView> hierarchy = new List<TreeView>();

            hierarchy = levels.Where(c => c.parentid == 0)
                            .Select(c => new TreeView()
                            {
                                key = c.key,
                                title = c.title,
                                code = c.code,
                                levelnumber = c.levelnumber,
                                parentid = c.parentid,
                                children = GetChildren(levels, c.key)
                            })
                            .ToList();


            HieararchyWalk(hierarchy);

            return hierarchy;
        }
        private void HieararchyWalk(List<TreeView> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    //Console.WriteLine(string.Format("{0} {1}", item.Id, item.Text));
                    HieararchyWalk(item.children);
                }
            }
        }
        public List<TreeView> GetChildren(List<TreeView> levels, int parentid)
        {
            return levels
                    .Where(c => c.parentid == parentid)
                    .Select(c => new TreeView()
                    {
                        key = c.key,
                        title = c.title,
                        code = c.code,
                        levelnumber = c.levelnumber,
                        parentid = c.parentid,
                        children = GetChildren(levels, c.key)
                    })
                    .ToList();
        }

        public string GetNode(string code)
        {
            throw new NotImplementedException();

        }

        public string GetNode(int id)
        {
            var list = new List<OC>();
            list = _context.OCs.ToList();
            var list2 = new List<OC>();
            list2.Add(list.FirstOrDefault(x => x.ID == id));
            var parentID = list.FirstOrDefault(x => x.ID == id).ParentID;
            foreach (var item in list)
            {
                if (parentID == 0)
                    break;
                if (parentID != 0)
                {
                    //add vao list1
                    list2.Add(list.FirstOrDefault(x => x.ID == parentID));
                }
                //cap nhat lai parentID
                parentID = list.FirstOrDefault(x => x.ID == parentID).ParentID;

            }
            return string.Join("->", list2.OrderBy(x => x.ParentID).Select(x => x.Name).ToArray());
        }
       
        public async Task<bool> IsExistsCode(int ID)
        {
            return await _context.OCs.AnyAsync(x => x.ID == ID);
        }

        public async Task<bool> Rename(TreeViewRename level)
        {
            var item = await _context.OCs.FindAsync(level.key);
            item.Name = level.title;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
