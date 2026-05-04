using MediatR;
using ViewModels.Specialization;

namespace Queries.Specialization
{
    public record GetEditSpecModel(Guid Id) : IRequest<SpecViewModel>;
}
