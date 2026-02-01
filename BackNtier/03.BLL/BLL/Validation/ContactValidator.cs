using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            // UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kullanıcı ID.");

            // ContactUserId
            RuleFor(x => x.ContactUserId)
                .NotEmpty().WithMessage("Kişi seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kişi ID.")
                .NotEqual(x => x.UserId).WithMessage("Kendinizi kişi olarak ekleyemezsiniz.");

            // AddedAt
            RuleFor(x => x.AddedAt)
                .NotEmpty().WithMessage("Eklenme zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Eklenme zamanı gelecekte olamaz.");
        }
    }
}
