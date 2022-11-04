using FluentValidation;

namespace SciMaterials.UI.BWASM.Models.Validations;

public class SignInFormValidator : AbstractValidator<SignInForm>
{
    public SignInFormValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            // TODO: fix message, prototype Include behavior
            .Length(8, 30).WithMessage("Password must be at least 8 characters long.");
    }
}