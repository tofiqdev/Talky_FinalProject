using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class GroupMemberValidator : AbstractValidator<GroupMember>
    {
        public GroupMemberValidator()
        {
            // GroupId
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Grup seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz grup ID.");

            // UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kullanıcı ID.");

            // IsAdmin
            RuleFor(x => x.IsAdmin)
                .NotNull().WithMessage("Admin durumu belirtilmelidir.");

            // IsMuted
            RuleFor(x => x.IsMuted)
                .NotNull().WithMessage("Sessize alma durumu belirtilmelidir.");

            // JoinedAt
            RuleFor(x => x.JoinedAt)
                .NotEmpty().WithMessage("Katılma zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Katılma zamanı gelecekte olamaz.");

            // Business rule: User cannot be member of same group twice
            // This would be checked in the service layer with database query
        }
    }
}
