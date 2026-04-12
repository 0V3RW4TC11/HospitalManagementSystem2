using Abstractions;
using Commands.Admin;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Handlers
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
                var admin = request.Data.Adapt<Domain.Entities.Admin>();
                await _unitOfWork.Admins.AddAsync(admin, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var userName = await _staffService.CreateStaffUsernameAsync(admin.FirstName, admin.LastName, ct);
                await _unitOfWork.IdentityProvider.CreateIdentityAsync(
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
                var admin = await _unitOfWork.Admins.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Admin>(request.Id), ct)
                    ?? throw new Exception("Admin not found with Id " + request.Id);
                await _unitOfWork.Admins.DeleteAsync(admin, ct);
                await _unitOfWork.IdentityProvider.DeleteIdentityAsync(admin.Id, ct);
            }, cancellationToken);
        }

        public async Task Handle(UpdateAdminCommand request, CancellationToken cancellationToken = default)
        {
            var admin = await _unitOfWork.Admins.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Admin>(request.Id), cancellationToken)
                    ?? throw new Exception("Admin not found with Id " + request.Id);

            admin.Title = request.Data.Title;
            admin.FirstName = request.Data.FirstName;
            admin.LastName = request.Data.LastName;
            admin.Gender = request.Data.Gender;
            admin.Address = request.Data.Address;
            admin.Phone = request.Data.Phone;
            admin.Email = request.Data.Email;
            admin.DateOfBirth = request.Data.DateOfBirth;

            await _unitOfWork.Admins.UpdateAsync(admin, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}