using System.ComponentModel.DataAnnotations;

namespace Presentation.Attributes
{
    internal class RequiredSpecializationsAttribute : ValidationAttribute
    {
        public RequiredSpecializationsAttribute()
        {
            ErrorMessage = "At least one specialization must be selected.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is ISet<Guid> collection && collection.Any())
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
