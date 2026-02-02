using Abstractions;
using Commands.Doctor;
using FluentValidation;
using Specifications.Doctor;

namespace Validation.Doctor
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c).SetValidator(new DoctorValidator());
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(c => c.Email).MustAsync(EmailMustBeUniqueForThisDoctor).WithMessage("This email is already used by another Doctor.");
            RuleFor(c => c.SpecializationIds).SetValidator(new DoctorSpecializationExistenceValidator(_unitOfWork));
        }

        private async Task<bool> EmailMustBeUniqueForThisDoctor(string email, CancellationToken ct)
        {
            return !await _unitOfWork.Doctors.AnyAsync(new DoctorByEmailSpec(email), ct);
        }
    }
}