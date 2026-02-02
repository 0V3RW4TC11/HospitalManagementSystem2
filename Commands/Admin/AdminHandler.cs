using Abstractions;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Admin
{
    public class AdminHandler :
        IRequestHandler<CreateAdminCommand>,
        IRequestHandler<DeleteAdminCommand>,
        IRequestHandler<UpdateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = request.Adapt<Domain.Entities.Admin>();
                await _unitOfWork.Admins.AddAsync(admin, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.IdentityProvider.IdentityManager.CreateAsync(admin, request.Password, ct);
            }, cancellationToken);
        }

        public async Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = await _unitOfWork.Admins.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Admin>(request.Id), ct)
                    ?? throw new Exception("Admin not found with Id " + request.Id);
                await _unitOfWork.Admins.DeleteAsync(admin, ct);
                await _unitOfWork.IdentityProvider.IdentityManager.DeleteAsync(admin.Id, ct);
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