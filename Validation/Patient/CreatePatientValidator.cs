using Abstractions;
using Commands.Patient;
using FluentValidation;
using Specifications.Patient;

namespace Validation.Patient
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePatientValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Dto).SetValidator(new PatientCorrectnessValidator());
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(c => c.Dto.Email).MustAsync(EmailMustBeUniqueForThisPatient);
        }

        private async Task<bool> EmailMustBeUniqueForThisPatient(
            string email,
            CancellationToken ct)
        {
            return !await _unitOfWork.Patients.AnyAsync(new PatientExistsWithEmailSpec(email), ct);
        }
    }
}
