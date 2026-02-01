using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using DAL.Concret;
using Entity.TableModel;
using Entity.DataTransferObject.MessageDTO;
using Entity.DataTransferObject.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concret
{
    public class UserManager: IUserService
    {
        private readonly IUserDAL _userDal;
        private readonly IMapper _mapper;

        public UserManager(IUserDAL userDal, IMapper mapper)
        {
            _userDal = userDal;
            _mapper = mapper;
        }

        public IResult Add(UserAddDTO userAddDTO)
        {
            var user = _mapper.Map<User>(userAddDTO);
            user.PasswordHash = userAddDTO.Password; // Password already hashed
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsOnline = false;
            user.Deleted = 0;

            var validateValidator = new UserValidator();
            var validationResult = validateValidator.Validate(user);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            _userDal.Add(user);
            return new SuccesResult("User registered successfully");
        }

        public IResult Delete(int id)
        {
            var userGet = _userDal.Get(x => x.Id == id);

            if (userGet is not null)
            {
                userGet.Deleted = id;
                _userDal.Update(userGet);
                return new SuccesResult("Deleted Succesfully");
            }
    
            return new ErrorResult("User Not found");


        }

        public IDataResult<UserListDTO> Get(int id)
        {
            var users = _userDal.Get(x => x.Deleted == 0 && x.Id == id);
            return new SuccessDataResult<UserListDTO>(_mapper.Map<UserListDTO>(users));
        }

        public IDataResult<List<UserListDTO>> GetAll()
        {
            var users = _userDal.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<UserListDTO>>(_mapper.Map<List<UserListDTO>>(users));
        }

        public IResult Update(UserUpdateDTO userUpdateDTO)
        {
            var userMapper = _mapper.Map<User>(userUpdateDTO);

            var validateValidator = new UserValidator();
            var validationResult = validateValidator.Validate(userMapper);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());


            var checkData = BusinessRules.Check(DuplicateUserName(userMapper));

            if (!checkData.IsSuccess)
                return checkData;

            _userDal.Update(userMapper);
            return new SuccesResult("Updated Succesfully");
        }



        public IResult DuplicateUserName(User user)
        {
            var userName = _userDal.Get(x => x.Name == user.Name && x.Deleted == 0);

            if (userName is not null)
            {
                return new ErrorResult("Duplicate Name");
            }

            return new SuccesResult();
        }
    }
}
