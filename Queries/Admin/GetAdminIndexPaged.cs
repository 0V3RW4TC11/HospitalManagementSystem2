using MediatR;
using Queries.Dtos.Admin;

namespace Queries.Admin
{
    public record GetAdminIndexPaged : IRequest<IEnumerable<AdminIndexItemDto>>;
}
