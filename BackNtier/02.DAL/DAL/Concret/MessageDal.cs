using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Concret;
using Core.Entities;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;
using Entity.TableModel;

namespace DAL.Concret
{
    public class MessageDal : BaseRepository<Message, ApplicationDbContext>, IMessageDal
    {
        public MessageDal(ApplicationDbContext context) : base(context)
        {
        }
    }

  
}
