using Core.Result.Abstrack;
using Entity.DataTransferObject.ContactDTO;
using System.Collections.Generic;

namespace BLL.Abstrack
{
    public interface IContactService
    {
        IDataResult<List<ContactListDTO>> GetAll();
        IDataResult<ContactListDTO> GetById(int id);
        IResult Add(ContactAddDTO contactAddDTO);
        IResult Update(ContactUpdateDTO contactUpdateDTO);
        IResult Delete(int id);
    }
}
