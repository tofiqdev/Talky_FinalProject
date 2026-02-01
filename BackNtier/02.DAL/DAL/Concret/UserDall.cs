using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;
using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Concret
{
    public class UserDal : BaseRepository<User, ApplicationDbContext>, IUserDAL
    {
        private readonly ApplicationDbContext _context;
        public UserDal(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        
        public User GetByUserId(int id)
        {
            return _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        }

        public List<User> UserGetAll()
        {
            return _context.ApplicationUsers.Where(u => u.Deleted == 0).ToList();
        }

        public List<Message> MessageGetAll()
        {
            return _context.Messages
                .Include(x => x.Sender)
                .Where(m => m.Deleted == 0)
                .ToList();
        }
        
        public List<Call> CallGetAll()
        {
            return _context.Calls
                .Include(x => x.Caller)
                .Where(c => c.Deleted == 0)
                .ToList();
        }
    }

    
}





