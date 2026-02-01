using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;
using Entity.TableModel;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Concret
{
    public class GroupMemberDAL : BaseRepository<GroupMember, ApplicationDbContext>, IGroupmemberDAL
    {
        private readonly ApplicationDbContext _context;
        public GroupMemberDAL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<GroupMember> UserGetAll()
        {
            return _context.GroupMembers.ToList();
        }

        public GroupMember GetByUserId(int id)
        {
            return _context.GroupMembers.FirstOrDefault(x => x.UserId == id);
        }

        public bool IsUserInGroup(int groupId, int userId)
        {
            return _context.GroupMembers.Any(x => x.GroupId == groupId && x.UserId == userId && x.Deleted == 0);
        }
    }
}
