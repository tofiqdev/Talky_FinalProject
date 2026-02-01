using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concret
{
    public class BlockedUserDAL : BaseRepository<BlockedUser, ApplicationDbContext>, IBlockedUserDAL
    {
        public BlockedUserDAL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
