using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class OCUserService : IOCUserService
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly IOCService _oCService;

        public OCUserService(DataContext context,IUserService userService,IOCService oCService)
        {
            _context = context;
            _userService = userService;
            _oCService = oCService;
        }

        public async Task<object> AddOrUpdate(int userid, int ocid)
        {

            try
            {
                var item = await _context.OCUsers.FirstOrDefaultAsync(x => x.OCID == ocid && x.UserID == userid);
                var user = await _context.Users.FindAsync(userid);

                if (item == null)
                {
                    var oc = new OCUser();
                    oc.OCID = ocid;
                    oc.UserID = userid;
                    user.OCID = ocid;
                    _context.OCUsers.Add(oc);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    user.OCID = 0;
                    _context.OCUsers.Remove(item);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<object> GetListUser(int ocid)
        {
            var users = await _context.Users.Select(x => new
            {
                x.ID,
                x.Username,
                RoleName=x.Role.Name,
                x.RoleID,
                Status = _context.OCUsers.Any(a=>a.UserID == x.ID && a.OCID == ocid)
            }).ToListAsync();
            return users;
        }
    }
}
