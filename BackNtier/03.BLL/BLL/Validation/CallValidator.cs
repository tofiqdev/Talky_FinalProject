using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class CallValidator : AbstractValidator<Call>
    {
        public CallValidator()
        {
            
            RuleFor(x => x.CallerId)
                .NotEmpty().WithMessage("Arayan kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz arayan kullanıcı ID.");

           
            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("Aranan kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz aranan kullanıcı ID.")
                .NotEqual(x => x.CallerId).WithMessage("Arayan ve aranan kullanıcı aynı olamaz.");

            
            RuleFor(x => x.CallType)
                .NotEmpty().WithMessage("Çağrı türü seçilmelidir.")
                .Must(x => x == "voice" || x == "video")
                .WithMessage("Çağrı türü sadece 'voice' veya 'video' olabilir.");

            
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Çağrı durumu belirtilmelidir.")
                .Must(x => x == "ringing" || x == "answered" ||
                           x == "in-progress" || x == "completed" ||
                           x == "missed" || x == "rejected" ||
                           x == "cancelled")
                .WithMessage("Geçersiz çağrı durumu.");

            
            RuleFor(x => x.StartedAt)
                .NotEmpty().WithMessage("Başlangıç zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Başlangıç zamanı gelecekte olamaz.");

            
            When(x => x.EndedAt.HasValue, () =>
            {
                RuleFor(x => x.EndedAt)
                    .GreaterThanOrEqualTo(x => x.StartedAt)
                    .WithMessage("Bitiş zamanı başlangıç zamanından önce olamaz.")
                    .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("Bitiş zamanı gelecekte olamaz.");
            });

           
            When(x => x.Duration.HasValue, () =>
            {
                RuleFor(x => x.Duration)
                    .GreaterThanOrEqualTo(0).WithMessage("Süre negatif olamaz.")
                    .LessThanOrEqualTo(86400).WithMessage("Süre 24 saati (86400 saniye) geçemez.");
            });

            
            RuleFor(x => x)
                .Must(x => !(x.Status == "completed" && !x.EndedAt.HasValue))
                .WithMessage("Tamamlanan çağrının bitiş zamanı olmalıdır.")
                .Must(x => !(x.Status == "completed" && !x.Duration.HasValue))
                .WithMessage("Tamamlanan çağrının süresi olmalıdır.");
        }
    }
}