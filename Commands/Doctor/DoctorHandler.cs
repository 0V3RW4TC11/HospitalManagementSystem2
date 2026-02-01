using Abstractions;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Doctor
{
    public class DoctorHandler :
        IRequestHandler<CreateDoctorCommand>,
        IRequestHandler<DeleteDoctorCommand>,
        IRequestHandler<UpdateDoctorCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DoctorSpecializationHelper _docSpecHandler;

        public DoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _docSpecHandler = new DoctorSpecializationHelper(_unitOfWork.DoctorSpecializations);
        }

        public async Task Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var doctor = request.Dto.Adapt<Domain.Entities.Doctor>();
                await _unitOfWork.Doctors.AddAsync(doctor, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _docSpecHandler.UpdateAsync(doctor.Id, request.SpecializationIds, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.IdentityProvider.IdentityManager.CreateAsync(doctor, request.Password, ct);
            }, cancellationToken);
        }

        public async Task Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var doctor = await _unitOfWork.Doctors.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Doctor>(request.Id), cancellationToken)
                    ?? throw new Exception("Doctor not found with Id " + request.Id);
                await _unitOfWork.Doctors.DeleteAsync(doctor, ct);
                await _unitOfWork.IdentityProvider.IdentityManager.DeleteAsync(doctor.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = request.Dto.Adapt<Domain.Entities.Doctor>();
            doctor.Id = request.Id;

            await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
            await _docSpecHandler.UpdateAsync(doctor.Id, request.SpecializationIds, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
