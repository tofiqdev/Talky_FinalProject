using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;

namespace DAL.Concret
{
    public class MovieRoomMessageDAL : BaseRepository<MovieRoomMessage, ApplicationDbContext>, IMovieRoomMessageDAL
    {
        private readonly ApplicationDbContext context;
        public MovieRoomMessageDAL(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
