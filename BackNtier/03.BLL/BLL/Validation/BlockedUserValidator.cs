using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class BlockedUserValidator : AbstractValidator<BlockedUser>
    {
        public BlockedUserValidator()
        {
            // UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kullanıcı ID.");

            // BlockedUserId
            RuleFor(x => x.BlockedUserId)
                .NotEmpty().WithMessage("Engellenecek kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz engellenecek kullanıcı ID.")
                .NotEqual(x => x.UserId).WithMessage("Kendinizi engelleyemezsiniz.");

            // BlockedAt
            RuleFor(x => x.BlockedAt)
                .NotEmpty().WithMessage("Engelleme zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Engelleme zamanı gelecekte olamaz.");
        }
    }
}
