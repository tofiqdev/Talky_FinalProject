using Core.Result.Abstrack;
using Entity.DataTransferObject.MovieRoomDTO;
using System.Collections.Generic;

namespace BLL.Abstrack
{
    public interface IMovieRoomService
    {
        IResult Add(MovieRoomAddDTO dto);
        IResult Update(MovieRoomUpdateDTO dto);
        IResult Delete(int id);
        IDataResult<List<MovieRoomListDTO>> GetAll();
        IDataResult<MovieRoomListDTO> Get(int id);
        IDataResult<List<MovieRoomListDTO>> GetActiveRooms();
        IResult JoinRoom(int roomId, int userId);
        IResult LeaveRoom(int roomId, int userId);
        IResult UpdatePlaybackState(int roomId, double currentTime, bool isPlaying);
    }
}
