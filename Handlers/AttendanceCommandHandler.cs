using Abstractions;
using Commands.Attendance;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Handlers
{
    public class AttendanceCommandHandler :
        IRequestHandler<CreateAttendanceCommand>,
        IRequestHandler<DeleteAttendanceCommand>,
        IRequestHandler<UpdateAttendanceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateAttendanceCommand request, CancellationToken cancellationToken = default)
        {
            var attendance = request.Data.Adapt<Domain.Entities.Attendance>();
            await _unitOfWork.Attendances.AddAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken = default)
        {
            var attendance = await _unitOfWork.Attendances.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Attendance>(request.Id), cancellationToken)
                ?? throw new Exception("Attendance not found with Id " + request.Id);
            await _unitOfWork.Attendances.DeleteAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken = default)
        {
            var attendance = await _unitOfWork.Attendances.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Attendance>(request.Id), cancellationToken)
                ?? throw new Exception("Attendance not found with Id " + request.Id);
            attendance.PatientId = request.Data.PatientId;
            attendance.DoctorId = request.Data.DoctorId;
            attendance.DateTime = request.Data.DateTime;
            attendance.Diagnosis = request.Data.Diagnosis;
            attendance.Remarks = request.Data.Remarks;
            attendance.Therapy = request.Data.Therapy;

            await _unitOfWork.Attendances.UpdateAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
