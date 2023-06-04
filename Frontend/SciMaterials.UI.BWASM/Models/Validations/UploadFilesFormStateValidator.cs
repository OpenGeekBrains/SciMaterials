using FluentValidation;

using SciMaterials.UI.BWASM.States.UploadFilesForm;

namespace SciMaterials.UI.BWASM.Models.Validations
{
    public class UploadFilesFormStateValidator : AbstractValidator<UploadFilesFormState>
    {
        public UploadFilesFormStateValidator()
        {
            RuleFor(x => x.ShortInfo)
                .NotEmpty();

            RuleFor(x => x.Category)
                .NotEmpty();

            RuleFor(x => x.Author)
                .NotEmpty();

            RuleFor(x => x.Files)
                .NotEmpty();
        }
    }
}