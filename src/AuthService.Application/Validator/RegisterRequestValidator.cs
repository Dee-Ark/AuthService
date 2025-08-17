using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validator
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[^\da-zA-Z]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Full name must contain only letters and spaces.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        }
    }
}
