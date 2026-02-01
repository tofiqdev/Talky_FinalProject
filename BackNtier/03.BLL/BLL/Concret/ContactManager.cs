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
    public class ContactManager : IContactService
    {
        private readonly IContactDAL _contactDAL;

        public ContactManager(IContactDAL contactDAL)
        {
            _contactDAL = contactDAL;
        }

        public IResult Add(Contact contact)
        {
            ContactValidator validator = new ContactValidator();
            ValidationResult result = validator.Validate(contact);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            var businessResult = BusinessRules.Check(
                CheckIfUserAddsHimself(contact.UserId, contact.ContactUserId),
                CheckIfContactAlreadyExists(contact.UserId, contact.ContactUserId)
            );

            if (businessResult != null)
            {
                return businessResult;
            }

            _contactDAL.Add(contact);
            return new SuccesResult("Kişi başarıyla eklendi.");
        }

        public IResult Delete(int id)
        {
            var contact = _contactDAL.Get(x => x.Id == id);
            if (contact == null)
            {
                return new ErrorResult("Kişi bulunamadı.");
            }

            _contactDAL.Delete(contact);
            return new SuccesResult("Kişi başarıyla silindi.");
        }

        public IDataResult<List<Contact>> GetAll()
        {
            var data = _contactDAL.GetAll().ToList();
            return new SuccessDataResult<List<Contact>>(data, "Kişiler listelendi.");
        }

        public IDataResult<Contact> GetById(int id)
        {
            var data = _contactDAL.Get(x => x.Id == id);
            if (data == null)
            {
                return new ErrorDataResult<Contact>("Kişi bulunamadı.");
            }
            return new SuccessDataResult<Contact>(data, "Kişi getirildi.");
        }

        public IResult Update(Contact contact)
        {
            ContactValidator validator = new ContactValidator();
            ValidationResult result = validator.Validate(contact);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            _contactDAL.Update(contact);
            return new SuccesResult("Kişi güncellendi.");
        }

        // Business Rules
        private IResult CheckIfUserAddsHimself(int userId, int contactUserId)
        {
            if (userId == contactUserId)
            {
                return new ErrorResult("Kendinizi kişi olarak ekleyemezsiniz.");
            }
            return new SuccesResult();
        }

        private IResult CheckIfContactAlreadyExists(int userId, int contactUserId)
        {
            var exists = _contactDAL.Get(x => x.UserId == userId && x.ContactUserId == contactUserId);
            if (exists != null)
            {
                return new ErrorResult("Bu kişi zaten eklenmiş.");
            }
            return new SuccesResult();
        }
    }
}
