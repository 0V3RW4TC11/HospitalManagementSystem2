using Abstractions;
using Commands.Doctor;
using FluentValidation;

namespace Validation.Doctor
{
    public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Dto).SetValidator(new DoctorCorrectnessValidator());
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(c => c.Dto.Email).MustAsync(EmailMustBeUniqueForThisDoctor);
            RuleFor(c => c.SpecializationIds).SetValidator(new DoctorSpecializationExistenceValidator(_unitOfWork));
        }

        private async Task<bool> EmailMustBeUniqueForThisDoctor(string email, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
