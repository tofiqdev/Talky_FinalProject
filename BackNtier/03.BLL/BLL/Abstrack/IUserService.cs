using Core.Result.Abstrack;
// using Entities.DataTransferObject.CategoryDTO; removed
using Entity.DataTransferObject.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IUserService
    {
        IResult Add(UserAddDTO UserAddDTO);
        IResult Update(UserUpdateDTO userUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<UserListDTO>> GetAll();
        IDataResult<UserListDTO> Get(int id);
    }
}
