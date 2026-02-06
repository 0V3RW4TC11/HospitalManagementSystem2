using Abstractions;
using Commands.Attendance;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Handlers
{
    public class AttendanceHandler :
        IRequestHandler<CreateAttendanceCommand>,
        IRequestHandler<DeleteAttendanceCommand>,
        IRequestHandler<UpdateAttendanceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendance = request.Adapt<Domain.Entities.Attendance>();
            await _unitOfWork.Attendances.AddAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _unitOfWork.Attendances.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Attendance>(request.Id), cancellationToken)
                ?? throw new Exception("Attendance not found with Id " + request.Id);
            await _unitOfWork.Attendances.DeleteAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendance = request.Adapt<Domain.Entities.Attendance>();
            await _unitOfWork.Attendances.UpdateAsync(attendance, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
