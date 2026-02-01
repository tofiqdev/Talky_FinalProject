using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class StoryValidator : AbstractValidator<Story>
    {
        public StoryValidator()
        {
            // UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz kullanıcı ID.");

            // ImageUrl
            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Hikaye görseli zorunludur.")
                .MaximumLength(1000).WithMessage("Görsel URL'si en fazla 1000 karakter olabilir.")
                .Must(BeValidUrl).WithMessage("Geçerli bir URL giriniz.");

            // Caption
            RuleFor(x => x.Caption)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            // CreatedAt
            RuleFor(x => x.CreatedAt)
                .NotEmpty().WithMessage("Oluşturulma zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Oluşturulma zamanı gelecekte olamaz.");

            // ExpiresAt
            RuleFor(x => x.ExpiresAt)
                .NotEmpty().WithMessage("Son kullanma zamanı belirtilmelidir.")
                .GreaterThan(x => x.CreatedAt)
                .WithMessage("Son kullanma zamanı oluşturulma zamanından sonra olmalıdır.");
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
