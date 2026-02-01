using Core.Concret;
using DAL.Database;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL.Abstrack; // Added for interface

namespace DAL.Concret
{
    public class GroupMessageDAL : BaseRepository<GroupMessage, ApplicationDbContext>, IGroupMessageDAL
    {
        public GroupMessageDAL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
