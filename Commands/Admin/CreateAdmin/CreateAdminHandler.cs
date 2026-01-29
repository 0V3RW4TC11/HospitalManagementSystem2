using Abstractions;
using MediatR;

namespace Commands.Admin.CreateAdmin
{
    public class CreateAdminHandler : IRequestHandler<CreateAdminCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Guid> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
