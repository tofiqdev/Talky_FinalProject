using Entity.TableModel;
using FluentValidation;

namespace BLL.Validation
{
    public class MovieRoomMessageValidator : AbstractValidator<MovieRoomMessage>
    {
        public MovieRoomMessageValidator()
        {
            RuleFor(x => x.MovieRoomId)
                .GreaterThan(0).WithMessage("Film otağı düzgün seçilməlidir.");

            RuleFor(x => x.SenderId)
                .GreaterThan(0).WithMessage("Göndərən istifadəçi düzgün seçilməlidir.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Mesaj boş ola bilməz.")
                .MaximumLength(5000).WithMessage("Mesaj maksimum 5000 simvol ola bilər.");
        }
    }
}
