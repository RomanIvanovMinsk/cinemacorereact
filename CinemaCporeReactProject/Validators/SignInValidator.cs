using CinemaCporeReactProject.Models;
using FluentValidation;

namespace CinemaCporeReactProject.Validators
{
    public class SignInValidator : AbstractValidator<SignInRequest>
    {
        public SignInValidator()
        {
            RuleFor(x=> x.Email).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().EmailAddress();
            RuleFor(x=> x.Password).NotEmpty().MaximumLength(200);
        }
    }
}
