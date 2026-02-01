using Core.Result.Abstrack;
using Entity.TableModel;
using Entity.DataTransferObject.StoryViewDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IStoryViewService
    {
        IDataResult<List<StoryView>> GetAll();
        IDataResult<StoryView> GetById(int id);
        IResult Add(StoryViewAddDTO storyViewAddDTO);
        IResult Update(StoryView storyView);
        IResult Delete(int id);
    }
}
