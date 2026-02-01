using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;

namespace DAL.Concret
{
    public class CallDAL : BaseRepository<Call, ApplicationDbContext>, ICallDAL
    {
        private readonly ApplicationDbContext context;
        public CallDAL(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
