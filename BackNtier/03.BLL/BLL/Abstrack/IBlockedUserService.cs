using Core.Result.Abstrack;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IBlockedUserService
    {
        IDataResult<List<BlockedUser>> GetAll();
        IDataResult<BlockedUser> GetById(int id);
        IResult Add(BlockedUser blockedUser);
        IResult Update(BlockedUser blockedUser);
        IResult Delete(int id);
    }
}
