using MediatR;

namespace Queries.Identity
{
    public record GetHmsUserIdFromCurrentUserQuery : IRequest<Guid>;
}
