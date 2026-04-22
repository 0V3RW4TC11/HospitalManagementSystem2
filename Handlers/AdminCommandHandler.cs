using Abstractions;
using Commands.Admin;
using Entities;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Handlers
{
    public class AdminCommandHandler :
        IRequestHandler<CreateAdminCommand>,
        IRequestHandler<DeleteAdminCommand>,
        IRequestHandler<UpdateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StaffService _staffService;

        public AdminCommandHandler(IUnitOfWork unitOfWork, StaffService staffService)
        {
            _unitOfWork = unitOfWork;
            _staffService = staffService;
        }

        public async Task Handle(CreateAdminCommand request, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = request.Data.Adapt<Admin>();
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

        public async Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = await _unitOfWork.Admins.SingleOrDefaultAsync(new EntityByIdSpec<Admin>(request.Id), ct)
                    ?? throw new NullReferenceException();

                await _unitOfWork.Admins.DeleteAsync(admin, ct);
                await _unitOfWork.IdentityService.DeleteIdentityAsync(admin.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdateAdminCommand request, CancellationToken cancellationToken = default)
        {
            var admin = request.Data.Adapt<Admin>();
            admin.Id = request.Id;

            await _unitOfWork.Admins.UpdateAsync(admin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}