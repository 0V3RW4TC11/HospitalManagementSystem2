using Abstractions;
using Commands.Patient;
using Entities;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Handlers
{
    public class PatientCommandHandler :
        IRequestHandler<CreatePatientCommand>,
        IRequestHandler<DeletePatientCommand>,
        IRequestHandler<UpdatePatientCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreatePatientCommand request, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var patient = request.Data.Adapt<Patient>();
                await _unitOfWork.Patients.AddAsync(patient, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.IdentityProvider.CreateIdentityAsync(
                    patient.Id,
                    patient.Email,
                    request.Password,
                    Constants.AuthRoles.Patient,
                    ct);
            }, cancellationToken);
        }

        public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var patient = await _unitOfWork.Patients.SingleOrDefaultAsync(new EntityByIdSpec<Patient>(request.Id), ct)
                    ?? throw new NullReferenceException();

                await _unitOfWork.Patients.DeleteAsync(patient, ct);
                await _unitOfWork.IdentityProvider.DeleteIdentityAsync(patient.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken = default)
        {
            var patient = request.Data.Adapt<Patient>();
            patient.Id = request.Id;

            await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}