using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers;
using Queries.Doctor;
using ViewModels.Doctor;
using ViewModels.Specialization;
using X.PagedList;

namespace Persistence.Handlers.Doctor
{
    public class DoctorQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        IRequestHandler<GetDoctorPagedModels, IPagedList<DoctorIndexViewModel>>,
        IRequestHandler<GetDoctorManageModel, ManageDoctorViewModel>,
        IRequestHandler<GetDoctorEditModel, EditDoctorViewModel>,
        IRequestHandler<GetDoctorProfileModel, ProfileDoctorViewModel>
    {
        private readonly UserQueryHandlerHelper _helper = new(context, userManager);

        public async Task<IPagedList<DoctorIndexViewModel>> Handle(GetDoctorPagedModels request, CancellationToken cancellationToken)
        {
            return await _helper.CreatePagedModelsAsync<Entities.Doctor, DoctorIndexViewModel>(
                d => d.FirstName,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

        public async Task<ManageDoctorViewModel> Handle(GetDoctorManageModel request, CancellationToken cancellationToken)
        {
            var doctor = await GetDoctorById(request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.Id);
            var specializationNames = await GetSpecializationNamesByDoctorId(request.Id, cancellationToken);

            return new ManageDoctorViewModel
            {
                Id = doctor.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                IsLockedOut = user.LockoutEnabled,
                Data = doctor.Adapt<DoctorDataViewModel>(),
                SpecializationNames = specializationNames
            };
        }

        public async Task<EditDoctorViewModel> Handle(GetDoctorEditModel request, CancellationToken cancellationToken)
        {
            var doctor = await GetDoctorById(request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.Id);

            var specializations = await context.Set<Entities.DoctorSpecialization>()
                .Where(ds => ds.DoctorId == request.Id)
                .Join(
                    context.Set<Entities.Specialization>(),
                    ds => ds.SpecializationId,
                    s => s.Id,
                    (ds, s) => s)
                .ProjectToType<SpecializationViewModel>()
                .ToListAsync(cancellationToken);

            var dsJson = new DoctorSpecializationsJson();
            dsJson.SetJsonFromSpecializations(specializations);

            return new EditDoctorViewModel
            {
                Id = doctor.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                Data = doctor.Adapt<DoctorDataViewModel>(),
                SpecializationsJson = dsJson
            };
        }

        public async Task<ProfileDoctorViewModel> Handle(GetDoctorProfileModel request, CancellationToken cancellationToken)
        {
            var doctor = await GetDoctorById(request.Id, cancellationToken);
            var user = await IdentityHelper.GetUserFromHmsIdAsync(userManager, request.Id);
            var specializationNames = await GetSpecializationNamesByDoctorId(request.Id, cancellationToken);

            return new ProfileDoctorViewModel
            {
                Id = doctor.Id,
                UserName = user.UserName ?? throw new ArgumentNullException(),
                Data = doctor.Adapt<DoctorDataViewModel>(),
                SpecializationNames = specializationNames
            };
        }

        private async Task<Entities.Doctor> GetDoctorById(Guid id, CancellationToken ct) =>
                                            await context.Set<Entities.Doctor>().SingleAsync(d => d.Id == id, ct);

        private async Task<IEnumerable<string>> GetSpecializationNamesByDoctorId(Guid id, CancellationToken ct) =>
            await context.Set<Entities.DoctorSpecialization>()
                .Where(ds => ds.DoctorId == id)
                .Join(
                    context.Set<Entities.Specialization>(),
                    ds => ds.SpecializationId,
                    s => s.Id,
                    (ds, s) => s.Name)
                .OrderBy(name => name)
                .ToListAsync(ct);
    }
}