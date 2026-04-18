using Abstractions;
using Commands.Specialization;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Handlers
{
    public class SpecializationCommandHandler :
        IRequestHandler<CreateSpecializationCommand>,
        IRequestHandler<DeleteSpecializationCommand>,
        IRequestHandler<UpdateSpecializationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecializationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateSpecializationCommand request, CancellationToken cancellationToken = default)
        {
            var specialization = request.Adapt<Domain.Entities.Specialization>();

            await _unitOfWork.Specializations.AddAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken = default)
        {
            var specialization = await _unitOfWork.Specializations.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Specialization>(request.Id), cancellationToken)
                ?? throw new NullReferenceException();
            
            await _unitOfWork.Specializations.DeleteAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken = default)
        {
            var specialization = request.Adapt<Domain.Entities.Specialization>();
            specialization.Id = request.Id;

            await _unitOfWork.Specializations.UpdateAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}