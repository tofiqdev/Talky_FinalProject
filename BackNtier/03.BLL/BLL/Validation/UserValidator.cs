using Entity.TableModel;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace BLL.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
           
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad soyad boş olamaz.")
                .Length(3, 100).WithMessage("Ad soyad 3-100 karakter arasında olmalıdır.")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$")
                .WithMessage("Ad soyad sadece harf ve boşluk içerebilir.");

          
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3-50 karakter arasında olmalıdır.")
                .Matches(@"^[a-zA-Z0-9_.]+$")
                .WithMessage("Kullanıcı adı sadece harf, rakam, nokta ve alt çizgi içerebilir.")
                .Must(BeUniqueUsername).WithMessage("Bu kullanıcı adı zaten kullanılıyor.");

            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(255).WithMessage("E-posta adresi en fazla 255 karakter olabilir.")
                .Must(BeUniqueEmail).WithMessage("Bu e-posta adresi zaten kullanılıyor.");

            
            When(x => !string.IsNullOrEmpty(x.PasswordHash), () =>
            {
                RuleFor(x => x.PasswordHash)
                    .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                    .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir.")
                    .Must(BeStrongPassword).WithMessage("Şifre yeterince güçlü değil.");
            });

            
            RuleFor(x => x.Avatar)
                .MaximumLength(500).WithMessage("Avatar URL'si en fazla 500 karakter olabilir.")
                .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.Avatar))
                .WithMessage("Geçerli bir URL giriniz.");

            
            RuleFor(x => x.Bio)
                .MaximumLength(2000).WithMessage("Biyografi en fazla 2000 karakter olabilir.");

            
            RuleFor(x => x.IsOnline)
                .NotNull().WithMessage("Çevrimiçi durumu belirtilmelidir.");

          
            When(x => x.LastSeen.HasValue, () =>
            {
                RuleFor(x => x.LastSeen)
                    .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("Son görülme zamanı gelecekte olamaz.");
            });

            
            RuleFor(x => x.CreatedAt)
                .NotEmpty().WithMessage("Oluşturulma zamanı belirtilmelidir.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Oluşturulma zamanı gelecekte olamaz.");

            
            RuleFor(x => x.UpdatedAt)
                .NotEmpty().WithMessage("Güncellenme zamanı belirtilmelidir.")
                .GreaterThanOrEqualTo(x => x.CreatedAt)
                .WithMessage("Güncellenme zamanı oluşturulma zamanından önce olamaz.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Güncellenme zamanı gelecekte olamaz.");
        }

        private bool BeUniqueUsername(string username)
        {
            
            return true; 
        }

        private bool BeUniqueEmail(string email)
        {
            
            return true; 
        }

        private bool BeStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return true;

            
            var hasUpper = new Regex(@"[A-Z]+");
            var hasLower = new Regex(@"[a-z]+");
            var hasDigit = new Regex(@"\d+");
            var hasSpecial = new Regex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+");

            return hasUpper.IsMatch(password) &&
                   hasLower.IsMatch(password) &&
                   hasDigit.IsMatch(password) &&
                   hasSpecial.IsMatch(password);
        }

        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}