using Abstractions;
using Commands.Patient.UpdatePatient;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Patient
{
    public class PatientHandler :
        IRequestHandler<CreatePatientCommand>,
        IRequestHandler<DeletePatientCommand>,
        IRequestHandler<UpdatePatientCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var patient = request.Adapt<Domain.Entities.Patient>();
                await _unitOfWork.Patients.AddAsync(patient, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.IdentityService.CreateIdentityAsync(
                    patient.Id,
                    patient.Email,
                    request.Password,
                    Constants.AuthRoles.Patient,
                    ct);
            }, cancellationToken);
        }

        public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var patient = await _unitOfWork.Patients.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Patient>(request.Id), ct)
                    ?? throw new Exception("Patient not found with Id " + request.Id);
                await _unitOfWork.Patients.DeleteAsync(patient, ct);
                await _unitOfWork.IdentityService.DeleteIdentityAsync(patient.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = request.Adapt<Domain.Entities.Patient>();

            await _unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}