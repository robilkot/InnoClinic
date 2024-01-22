using FluentValidation;

namespace ServicesService.Presentation.Models.Validators
{
    public class ServiceModelValidator : AbstractValidator<ClientServiceModel>
    {
        public ServiceModelValidator()
        {
            RuleFor(s => s.Name)
                .NotNull()
                .Length(2,256)
                .WithMessage("Length of name must be 2 to 256 symbols")
                .Must((s) =>
                {
                    return !s.Contains(',') && !s.Contains(';') && !s.Contains('.');
                })
                .WithMessage("Name must not contain commas, semicolons or dots");

            RuleFor(s => s.CategoryId).NotNull();
            RuleFor(s => s.Price).NotNull();
            RuleFor(s => s.SpecializationId).NotNull();
            RuleFor(s => s.IsActive).NotNull();
        }
    }
}
