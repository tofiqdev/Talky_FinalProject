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
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Concret
{
    public class MessageDal : BaseRepository<Message, ApplicationDbContext>, IMessageDal
    {
        public MessageDal(ApplicationDbContext context) : base(context)
        {
        }

        public override List<Message> GetAll(Expression<Func<Message, bool>>? filter = null)
        {
            return filter == null
                ? _context.Set<Message>().Include(m => m.Sender).Include(m => m.Receiver).ToList()
                : _context.Set<Message>().Where(filter).Include(m => m.Sender).Include(m => m.Receiver).ToList();
        }

        public override Message? Get(Expression<Func<Message, bool>> filter)
        {
            return _context.Set<Message>().Where(filter).Include(m => m.Sender).Include(m => m.Receiver).FirstOrDefault();
        }
    }
}
