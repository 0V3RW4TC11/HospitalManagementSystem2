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
        private readonly StaffService _staffService;
        private readonly DoctorSpecializationHelper _docSpecHelper;

        public DoctorHandler(IUnitOfWork unitOfWork, StaffService staffService)
        {
            _unitOfWork = unitOfWork;
            _staffService = staffService;
            _docSpecHelper = new DoctorSpecializationHelper(_unitOfWork.DoctorSpecializations);
        }

        public async Task Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var doctor = request.Adapt<Domain.Entities.Doctor>();
                await _unitOfWork.Doctors.AddAsync(doctor, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _docSpecHelper.UpdateAsync(doctor.Id, request.SpecializationIds, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var userName = await _staffService.CreateStaffUsernameAsync(doctor.FirstName, doctor.LastName, ct);
                await _unitOfWork.IdentityService.CreateIdentityAsync(
                    doctor.Id,
                    userName,
                    request.Password,
                    Constants.AuthRoles.Doctor,
                    ct);
            }, cancellationToken);
        }

        public async Task Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var doctor = await _unitOfWork.Doctors.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Doctor>(request.Id), cancellationToken)
                    ?? throw new Exception("Doctor not found with Id " + request.Id);
                await _unitOfWork.Doctors.DeleteAsync(doctor, ct);
                await _unitOfWork.IdentityService.DeleteIdentityAsync(doctor.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = request.Adapt<Domain.Entities.Doctor>();

            await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
            await _docSpecHelper.UpdateAsync(doctor.Id, request.SpecializationIds, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}