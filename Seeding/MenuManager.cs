using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeding.Helpers;

namespace Seeding
{
    internal class MenuManager
    {
        private readonly MenuActions _actions;
        private readonly RepositoryDbContext _context;

        public MenuManager(IServiceProvider services)
        {
            _actions = new MenuActions(services);
            _context = services.GetRequiredService<RepositoryDbContext>();
        }

        public async Task<Dictionary<string, Func<Task>>> GetSeedingMenuAsync()
        {
            var hasData = await ContextHelper.HasDataAsync(_context);

            if (hasData)
            {
                return new Dictionary<string, Func<Task>>
                {
                    ["Clear All Database Data"] = _actions.ResetDatabaseAsync,
                    ["Exit"] = () => Task.CompletedTask
                };
            }
            else
            {
                return new Dictionary<string, Func<Task>>
                {
                    ["Seed Admins"] = _actions.SeedAdminsAsync,
                    ["Seed Patients"] = _actions.SeedPatientsAsync,
                    ["Seed Doctors"] = _actions.SeedDoctorsAsync,
                    ["Seed All"] = _actions.SeedAllAsync,
                    ["Exit"] = () => Task.CompletedTask
                };
            }
        }
    }
}