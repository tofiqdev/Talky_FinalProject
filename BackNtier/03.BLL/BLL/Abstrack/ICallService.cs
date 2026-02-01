using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Result.Abstrack;
// using Entities.DataTransferObject.CategoryDTO; removed
using Entity.TableModel;
// using static Entities.DataTransferObject.CategoryDTO.MessageListDTOCallListDTO; removed
using Entity.DataTransferObject.CallDTO;

namespace BLL.Abstrack
{
    public interface ICallService
    {
        IResult Add(CallAddDTO callAddDTO);
        IResult Update(CallUpdateDTO callUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<CallListDTO>> GetAll();
        IDataResult<CallListDTO> Get(int id);
    }

    
}
