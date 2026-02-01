using Core.Result.Abstrack;
using Entity.TableModel;
using Entity.DataTransferObject.StoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IStoryService
    {
        IDataResult<List<Story>> GetAll();
        IDataResult<Story> Get(int id);
        IDataResult<Story> GetById(int id);
        IDataResult<StoryListDTO> Add(StoryAddDTO storyAddDTO);
        IResult Update(Story story);
        IResult Delete(int id);
    }
}
