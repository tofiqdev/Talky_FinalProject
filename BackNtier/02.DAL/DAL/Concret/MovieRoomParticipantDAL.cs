using Core.Concret;
using DAL.Abstrack;
using DAL.Database;
using Entity.TableModel;

namespace DAL.Concret
{
    public class MovieRoomParticipantDAL : BaseRepository<MovieRoomParticipant, ApplicationDbContext>, IMovieRoomParticipantDAL
    {
        private readonly ApplicationDbContext context;
        public MovieRoomParticipantDAL(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
