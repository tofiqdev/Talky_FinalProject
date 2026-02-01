using Core.Abstrack;
using Entity.TableModel;
using Entity.TableModel;
using System.Collections.Generic;

namespace DAL.Abstrack
{
    public interface IGroupmemberDAL : IBaseRepository<GroupMember>
    {
        List<GroupMember> UserGetAll();
        GroupMember GetByUserId(int id);
        bool IsUserInGroup(int groupId, int userId); // <-- bu yox idi
    }
}
