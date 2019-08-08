using CinemaCporeReactProject.Models;
using FluentValidation;

namespace CinemaCporeReactProject.Validators
{
    public class SignUpValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.Email).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
        }
    }
}
