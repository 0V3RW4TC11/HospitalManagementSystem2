using Abstractions;
using Mapster;
using MediatR;

namespace Commands.Admin.UpdateAdmin
{
    public class UpdateAdminHandler : IRequestHandler<UpdateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAdminHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = request.Dto.Adapt<Domain.Entities.Admin>();
            admin.Id = request.Id;

            await _unitOfWork.Admins.UpdateAsync(admin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
