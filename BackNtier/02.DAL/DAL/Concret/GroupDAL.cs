using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;





namespace DAL.Concret
{
    public class GroupDAL
    : BaseRepository<Group, ApplicationDbContext>, IGroupDAL
    {
        private readonly ApplicationDbContext context;
        public GroupDAL(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
    
}
