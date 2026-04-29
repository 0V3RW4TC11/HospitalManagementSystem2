using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Handlers.Helpers
{
    internal static class DoctorQueryHelper
    {
        public static async Task<IEnumerable<string>> GetSpecializationNamesByDoctorId(
            HmsDbContext context,
            Guid id, 
            CancellationToken ct) =>
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
