using Abstractions;
using Mapster;
using MediatR;

namespace Commands.Admin.CreateAdmin
{
    public class CreateAdminHandler : IRequestHandler<CreateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RunInTransactionAsync(async (ct) =>
            {
                var admin = request.Dto.Adapt<Domain.Entities.Admin>();
                await _unitOfWork.Admins.AddAsync(admin, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.IdentityProvider.CreateAsync(admin, request.Password, ct);
            }, cancellationToken);
        }
    }
}
