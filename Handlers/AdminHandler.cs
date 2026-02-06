using Abstractions;
using Commands.Admin;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Handlers
{
    public class AdminHandler :
        IRequestHandler<CreateAdminCommand>,
        IRequestHandler<DeleteAdminCommand>,
        IRequestHandler<UpdateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StaffService _staffService;

        public AdminHandler(IUnitOfWork unitOfWork, StaffService staffService)
        {
            _unitOfWork = unitOfWork;
            _staffService = staffService;
        }

        public async Task Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = request.Adapt<Domain.Entities.Admin>();
                await _unitOfWork.Admins.AddAsync(admin, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var userName = await _staffService.CreateStaffUsernameAsync(admin.FirstName, admin.LastName, ct);
                await _unitOfWork.IdentityService.CreateIdentityAsync(
                    admin.Id, 
                    userName,
                    request.Password,
                    Constants.AuthRoles.Admin,
                    ct);
            }, cancellationToken);
        }

        public async Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = await _unitOfWork.Admins.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Admin>(request.Id), ct)
                    ?? throw new Exception("Admin not found with Id " + request.Id);
                await _unitOfWork.Admins.DeleteAsync(admin, ct);
                await _unitOfWork.IdentityService.DeleteIdentityAsync(admin.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = request.Adapt<Domain.Entities.Admin>();

            await _unitOfWork.Admins.UpdateAsync(admin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}