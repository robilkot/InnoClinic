using FluentValidation;
using ServicesService.Domain.Entities;

namespace ServicesService.Presentation.Models.Validators
{
    public class SpecializationModelValidator : AbstractValidator<ClientSpecializationModel>
    {
        public SpecializationModelValidator()
        {
            RuleFor(s => s.Name)
                .Must(str =>
                {
                    if (str != null)
                    {
                        return str.Length >= 2 && str.Length <= 255 && !str.Contains(',');
                    }
                    else
                    {
                        return true;
                    }
                })
                .WithMessage("Name must not contain commas and must be 2 to 255 symbols long");

        }
    }
}
