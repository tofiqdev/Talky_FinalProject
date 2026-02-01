using Core.Result.Abstrack;
using Entity.DataTransferObject.MovieRoomDTO;
using System.Collections.Generic;

namespace BLL.Abstrack
{
    public interface IMovieRoomMessageService
    {
        IResult Add(MovieRoomMessageAddDTO dto, int senderId);
        IDataResult<List<MovieRoomMessageListDTO>> GetRoomMessages(int roomId);
    }
}
