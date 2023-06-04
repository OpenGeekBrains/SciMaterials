using FluentValidation;

namespace SciMaterials.UI.BWASM.Models.Validations;

public class SignUpFormValidator : AbstractValidator<SignUpForm>
{
    public SignUpFormValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(255).WithMessage("Name length can't be more than 255.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            // TODO: fix message, prototype Include behavior
            .Length(8, 30).WithMessage("Password must be at least 8 characters long.");
    }
}