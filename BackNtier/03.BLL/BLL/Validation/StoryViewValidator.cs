using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class StoryViewValidator : AbstractValidator<StoryView>
    {
        public StoryViewValidator()
        {
            // StoryId
            RuleFor(x => x.StoryId)
                .NotEmpty().WithMessage("Hikaye seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz hikaye ID.");

            // UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kullanıcı ID.");

            // ViewedAt
            RuleFor(x => x.ViewedAt)
                .NotEmpty().WithMessage("Görüntüleme zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Görüntüleme zamanı gelecekte olamaz.");
        }
    }
}
