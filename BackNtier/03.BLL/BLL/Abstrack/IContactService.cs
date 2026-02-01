using Core.Result.Abstrack;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IContactService
    {
        IDataResult<List<Contact>> GetAll();
        IDataResult<Contact> GetById(int id);
        IResult Add(Contact contact);
        IResult Update(Contact contact);
        IResult Delete(int id);
    }
}
