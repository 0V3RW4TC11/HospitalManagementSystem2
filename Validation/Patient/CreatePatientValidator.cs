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

            RuleFor(c => c).SetValidator(new PatientValidator());
            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
            RuleFor(c => c.Email)
                .MustAsync(EmailMustBeUniqueForThisPatient)
                .WithMessage("This email is already used by another Patient");
        }

        private async Task<bool> EmailMustBeUniqueForThisPatient(string email, CancellationToken ct)
        {
            return !await _unitOfWork.Patients.AnyAsync(new PatientByEmailSpec(email), ct);
        }
    }
}