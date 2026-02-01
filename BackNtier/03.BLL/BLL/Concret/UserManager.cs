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
            // Get fresh entity from database (not tracked)
            var existingUser = _userDal.Get(x => x.Id == userUpdateDTO.Id && x.Deleted == 0);
            
            if (existingUser == null)
                return new ErrorResult("User not found");

            // Check duplicate name (excluding current user)
            var checkData = BusinessRules.Check(DuplicateUserName(userUpdateDTO.Id, userUpdateDTO.Name));
            if (!checkData.IsSuccess)
                return checkData;

            // Update properties
            existingUser.Name = userUpdateDTO.Name;
            existingUser.Username = userUpdateDTO.Username;
            existingUser.Email = userUpdateDTO.Email;
            existingUser.Avatar = userUpdateDTO.Avatar;
            existingUser.Bio = userUpdateDTO.Bio;
            existingUser.IsOnline = userUpdateDTO.IsOnline;
            existingUser.LastSeen = userUpdateDTO.LastSeen;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Validate
            var validateValidator = new UserValidator();
            var validationResult = validateValidator.Validate(existingUser);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            _userDal.Update(existingUser);
            return new SuccesResult("Updated Succesfully");
        }

        public IResult DuplicateUserName(int userId, string name)
        {
            var isDuplicate = _userDal.GetAll(x => x.Name == name && x.Deleted == 0 && x.Id != userId).Any();

            if (isDuplicate)
            {
                return new ErrorResult("Duplicate Name");
            }

            return new SuccesResult();
        }
    }
}
