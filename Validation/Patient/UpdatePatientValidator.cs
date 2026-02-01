using Abstractions;
using Commands.Patient.UpdatePatient;
using FluentValidation;
using Specifications.Patient;
using Validation.Shared;

namespace Validation.Patient
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePatientValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Dto).SetValidator(new PatientCorrectnessValidator());
            RuleFor(c => c.Id).SetValidator(new EntityValidator<Domain.Entities.Patient>(_unitOfWork.Patients));
            RuleFor(c => c.Dto.Email)
                .MustAsync(EmailMustBeUniqueForThisPatient)
                .WithMessage("This email is already used by another Patient");
        }

        private async Task<bool> EmailMustBeUniqueForThisPatient(UpdatePatientCommand command, string email, CancellationToken cancellationToken)
        {
            Guid idFromEmail = await _unitOfWork.Patients.SingleOrDefaultAsync(new GetPatientIdByEmailSpec(email), cancellationToken);

            return (idFromEmail == Guid.Empty) || (idFromEmail == command.Id);
        }
    }
}
