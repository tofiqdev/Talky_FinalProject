using Entity.TableModel;
using FluentValidation;

namespace BLL.Validation
{
    public class GroupValidator : AbstractValidator<Group>
    {
        public GroupValidator()
        {
            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Qrup adı boş ola bilməz.")
                .Length(3, 250).WithMessage("Qrup adı 3–250 simvol arasında olmalıdır.")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s\.\-_,]+$")
                .WithMessage("Qrup adı yalnız hərf, rəqəm və icazəli simvollar ola bilər.");

            // Description
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Açıqlama maksimum 1000 simvol ola bilər.");

            // Avatar (url və ya file adı ola bilər)
            RuleFor(x => x.Avatar)
                .MaximumLength(500)
                .WithMessage("Avatar uzunluğu maksimum 500 simvol ola bilər.");

            // CreatedById
            RuleFor(x => x.CreatedById)
                .GreaterThan(0)
                .WithMessage("Qrup yaradan istifadəçi düzgün seçilməlidir.");

            // CreatedAt
            RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Yaradılma tarixi gələcəkdə ola bilməz.");

            // UpdatedAt
            RuleFor(x => x.UpdatedAt)
                .GreaterThanOrEqualTo(x => x.CreatedAt)
                .WithMessage("Yenilənmə tarixi yaradılma tarixindən əvvəl ola bilməz.");
        }
    }
}
