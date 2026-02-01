using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;

namespace DAL.Concret
{
    public class MovieRoomDAL : BaseRepository<MovieRoom, ApplicationDbContext>, IMovieRoomDAL
    {
        private readonly ApplicationDbContext context;
        public MovieRoomDAL(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
