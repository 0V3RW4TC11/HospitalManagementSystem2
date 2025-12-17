using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seeding
{
    internal interface IDataSeeder
    {
        Task SeedAuthenticationRolesAsync();

        Task SeedDoctorSpecializationsAsync();

        Task SeedAdminsAsync();

        Task SeedDoctorsAsync();

        Task SeedPatientsAsync();
    }
}
