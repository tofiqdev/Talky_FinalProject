using Core.Abstrack;
using Entity.TableModel;

namespace DAL.Abstrack
{
    public interface IUserDAL : IBaseRepository<User>
    {
        List<User> UserGetAll();
        User GetByUserId(int id);
        List<Message> MessageGetAll();
        List<Call> CallGetAll();
    }
}