using FluentValidation;
using ServicesService.Domain.Entities;

namespace ServicesService.Presentation.Models.Validators
{
    public class SpecializationModelValidator : AbstractValidator<ClientSpecializationModel>
    {
        public SpecializationModelValidator()
        {
            RuleFor(s => s.Name)
                .NotNull()
                .Length(2,256)
                .WithMessage("Length of name must be 2 to 256 symbols")
                .Must((s) =>
                {
                    return !s.Contains(';') && !s.Contains('.');
                })
                .WithMessage("Name must not contain semicolons or dots");

            RuleFor(s => s.IsActive).NotNull();
        }
    }
}
