using Core.Result.Abstrack;
using Entity.DataTransferObject.MessageDTO;
using System.Collections.Generic;

namespace BLL.Abstrack
{
    public interface IMessageService
    {
        IResult Add(MessageAddDTO messageAddDTO);
        IResult Update(MessageUpdateDTO messageUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<MessageListDTO>> GetAll();
        IDataResult<MessageListDTO> Get(int id);
    }
}