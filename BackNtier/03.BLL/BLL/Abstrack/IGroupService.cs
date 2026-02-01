using Core.Result.Abstrack;
using Entity.DataTransferObject.GroupDTO;
using Entity.DataTransferObject.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IGroupService
    {
        IResult Add(GroupAddDTO GroupAddDTO);
        IResult Update(GroupUpdateDTO GroupUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<GroupListDTO>> GetAll();
        IDataResult<GroupListDTO> Get(int id);
    }

   
}
