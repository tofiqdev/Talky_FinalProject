using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.TableModel;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concret
{
    public class BlockedUserManager : IBlockedUserService
    {
        private readonly IBlockedUserDAL _blockedUserDAL;

        public BlockedUserManager(IBlockedUserDAL blockedUserDAL)
        {
            _blockedUserDAL = blockedUserDAL;
        }

        public IResult Add(BlockedUser blockedUser)
        {
            BlockedUserValidator validator = new BlockedUserValidator();
            ValidationResult result = validator.Validate(blockedUser);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            var businessResult = BusinessRules.Check(
                CheckIfUserBlocksHimself(blockedUser.UserId, blockedUser.BlockedUserId),
                CheckIfAlreadyBlocked(blockedUser.UserId, blockedUser.BlockedUserId)
            );

            if (businessResult != null)
            {
                return businessResult;
            }

            _blockedUserDAL.Add(blockedUser);
            return new SuccesResult("Kullanıcı başarıyla engellendi.");
        }

        public IResult Delete(int id)
        {
            var blockedUser = _blockedUserDAL.Get(x => x.Id == id);
            if (blockedUser == null)
            {
                return new ErrorResult("Engel kaydı bulunamadı.");
            }

            _blockedUserDAL.Delete(blockedUser);
            return new SuccesResult("Engel başarıyla kaldırıldı.");
        }

        public IDataResult<List<BlockedUser>> GetAll()
        {
            var data = _blockedUserDAL.GetAll().ToList();
            return new SuccessDataResult<List<BlockedUser>>(data, "Engellenen kullanıcılar listelendi.");
        }

        public IDataResult<BlockedUser> GetById(int id)
        {
            var data = _blockedUserDAL.Get(x => x.Id == id);
            if (data == null)
            {
                return new ErrorDataResult<BlockedUser>("Engel kaydı bulunamadı.");
            }
            return new SuccessDataResult<BlockedUser>(data, "Engel kaydı getirildi.");
        }

        public IResult Update(BlockedUser blockedUser)
        {
            BlockedUserValidator validator = new BlockedUserValidator();
            ValidationResult result = validator.Validate(blockedUser);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            _blockedUserDAL.Update(blockedUser);
            return new SuccesResult("Engel kaydı güncellendi.");
        }

        // Business Rules
        private IResult CheckIfUserBlocksHimself(int userId, int blockedUserId)
        {
            if (userId == blockedUserId)
            {
                return new ErrorResult("Kendinizi engelleyemezsiniz.");
            }
            return new SuccesResult();
        }

        private IResult CheckIfAlreadyBlocked(int userId, int blockedUserId)
        {
            var exists = _blockedUserDAL.Get(x => x.UserId == userId && x.BlockedUserId == blockedUserId);
            if (exists != null)
            {
                return new ErrorResult("Bu kullanıcı zaten engellenmiş.");
            }
            return new SuccesResult();
        }
    }
}
