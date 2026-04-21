using MediatR;

namespace Queries.Identity
{
    public record GetHmsUserIdQuery : IRequest<Guid>;
}
