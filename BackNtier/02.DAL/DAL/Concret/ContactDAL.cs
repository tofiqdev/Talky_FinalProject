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
    public class ContactDAL : BaseRepository<Contact, ApplicationDbContext>, IContactDAL
    {
        public ContactDAL(ApplicationDbContext context) : base(context)
        {
        }
    }
}
