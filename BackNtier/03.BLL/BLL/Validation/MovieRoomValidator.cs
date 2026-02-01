using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class MovieRoomValidator : AbstractValidator<MovieRoom>
    {
        public MovieRoomValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Film otağı adı boş ola bilməz.")
                .Length(3, 250).WithMessage("Film otağı adı 3–250 simvol arasında olmalıdır.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Açıqlama maksimum 1000 simvol ola bilər.");

            RuleFor(x => x.YouTubeUrl)
                .NotEmpty().WithMessage("YouTube URL boş ola bilməz.")
                .MaximumLength(500).WithMessage("URL çox uzundur.");

            RuleFor(x => x.YouTubeVideoId)
                .NotEmpty().WithMessage("YouTube video ID boş ola bilməz.")
                .Length(11).WithMessage("YouTube video ID 11 simvol olmalıdır.");

            RuleFor(x => x.CreatedById)
                .GreaterThan(0).WithMessage("Yaradan istifadəçi düzgün seçilməlidir.");

            RuleFor(x => x.CurrentTime)
                .GreaterThanOrEqualTo(0).WithMessage("Video vaxtı mənfi ola bilməz.");
        }
    }
}
