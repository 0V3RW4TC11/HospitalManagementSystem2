using Abstractions;
using Commands.Doctor;
using FluentValidation;
using Specifications.Doctor;

namespace Validation.Doctor
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Data).SetValidator(new DoctorValidator());
            RuleFor(c => c.Data.SpecializationIds).SetValidator(new DoctorSpecializationValidator(_unitOfWork));
            RuleFor(c => c.Data.Email).MustAsync(EmailMustBeUniqueForThisDoctor).WithMessage("This email is already used by another Doctor.");
        }

        private async Task<bool> EmailMustBeUniqueForThisDoctor(UpdateDoctorCommand command, string email, CancellationToken ct)
        {
            Guid idFromEmail = await _unitOfWork.Doctors.SingleOrDefaultAsync(new DoctorIdByEmailSpec(email), ct);
            return (idFromEmail == Guid.Empty) || (idFromEmail == command.Id);
        }
    }
}