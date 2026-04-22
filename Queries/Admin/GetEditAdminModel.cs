using MediatR;
using ViewModels.Admin;

namespace Queries.Admin
{
    public record GetEditAdminModel(Guid Id) : IRequest<EditViewModel>;
}
