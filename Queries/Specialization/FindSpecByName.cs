using MediatR;
using ViewModels.Specialization;

namespace Queries.Specialization
{
    public record FindSpecsByName(string Name) : IRequest<IEnumerable<SpecializationViewModel>>;
}
