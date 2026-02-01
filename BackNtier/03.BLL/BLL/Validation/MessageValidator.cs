using Entity.TableModel;
using FluentValidation;
using System;

namespace BLL.Validation
{
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderen kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz gönderen kullanıcı ID.");

            
            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("Alıcı kullanıcı seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz alıcı kullanıcı ID.")
                .NotEqual(x => x.SenderId).WithMessage("Gönderen ve alıcı kullanıcı aynı olamaz.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Mesaj içeriği boş olamaz.")
                .MinimumLength(1).WithMessage("Mesaj içeriği en az 1 karakter olmalıdır.")
                .MaximumLength(5000).WithMessage("Mesaj içeriği en fazla 5000 karakter olabilir.")
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Mesaj içeriği sadece boşluklardan oluşamaz.");

           
            RuleFor(x => x.IsRead)
                .NotNull().WithMessage("Okunma durumu belirtilmelidir.");

           
            RuleFor(x => x.SentAt)
                .NotEmpty().WithMessage("Gönderim zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Gönderim zamanı gelecekte olamaz.");

            
            When(x => x.IsRead, () =>
            {
                RuleFor(x => x.ReadAt)
                    .NotNull().WithMessage("Okunma zamanı belirtilmelidir.")
                    .GreaterThanOrEqualTo(x => x.SentAt)
                    .WithMessage("Okunma zamanı gönderim zamanından önce olamaz.")
                    .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("Okunma zamanı gelecekte olamaz.");
            });

            
            RuleFor(x => x)
                .Must(x => !(x.IsRead && !x.ReadAt.HasValue))
                .WithMessage("Okunmuş mesajın okunma zamanı olmalıdır.")
                .Must(x => !(!x.IsRead && x.ReadAt.HasValue))
                .WithMessage("Okunmamış mesajın okunma zamanı olamaz.");
        }
    }
}