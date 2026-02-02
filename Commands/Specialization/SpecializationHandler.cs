using Abstractions;
using Mapster;
using MediatR;
using Specifications.Entity;

namespace Commands.Specialization
{
    public class SpecializationHandler :
        IRequestHandler<CreateSpecializationCommand>,
        IRequestHandler<DeleteSpecializationCommand>,
        IRequestHandler<UpdateSpecializationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecializationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = request.Adapt<Domain.Entities.Specialization>();
            await _unitOfWork.Specializations.AddAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.SingleOrDefaultAsync(new EntityByIdSpec<Domain.Entities.Specialization>(request.Id), cancellationToken)
                ?? throw new Exception("Patient not found with Id " + request.Id);
            await _unitOfWork.Specializations.DeleteAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = request.Adapt<Domain.Entities.Specialization>();
            await _unitOfWork.Specializations.UpdateAsync(specialization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}