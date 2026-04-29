using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Handlers.Base;
using Persistence.Helpers;
using Queries.Doctor;
using ViewModels.Doctor;
using ViewModels.Specialization;

namespace Persistence.Handlers.Doctor
{
    public class DoctorQueryHandler(HmsDbContext context, UserManager<IdentityUser> userManager) :
        PagedQueryHandlerBase<Entities.Doctor, DoctorIndexViewModel>(context, x => x.FirstName),
        IRequestHandler<GetDoctorManageModel, ManageDoctorViewModel>,
        IRequestHandler<GetDoctorEditModel, EditDoctorViewModel>,
        IRequestHandler<GetDoctorProfileModel, ProfileDoctorViewModel>
    {
        private readonly HmsDbContext _context = context;

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

            var specializations = await _context.Set<Entities.DoctorSpecialization>()
                .Where(ds => ds.DoctorId == request.Id)
                .Join(
                    _context.Set<Entities.Specialization>(),
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
                                            await _context.Set<Entities.Doctor>().SingleAsync(d => d.Id == id, ct);

        private async Task<IEnumerable<string>> GetSpecializationNamesByDoctorId(Guid id, CancellationToken ct) =>
            await _context.Set<Entities.DoctorSpecialization>()
                .Where(ds => ds.DoctorId == id)
                .Join(
                    _context.Set<Entities.Specialization>(),
                    ds => ds.SpecializationId,
                    s => s.Id,
                    (ds, s) => s.Name)
                .OrderBy(name => name)
                .ToListAsync(ct);
    }
}