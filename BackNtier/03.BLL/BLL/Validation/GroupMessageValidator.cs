using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class GroupMessageValidator : AbstractValidator<GroupMessage>
    {
        public GroupMessageValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz Group ID.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderen kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz Sender ID.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Mesaj içeriği boş olamaz.")
                .MinimumLength(1).WithMessage("Mesaj en az 1 karakter olmalıdır.")
                .MaximumLength(5000).WithMessage("Mesaj 5000 karakteri geçemez.");

            RuleFor(x => x.SentAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Gönderim zamanı gelecekte olamaz.");

            // IsRead validation removed because property does not exist in Entity

        }
    }
}
