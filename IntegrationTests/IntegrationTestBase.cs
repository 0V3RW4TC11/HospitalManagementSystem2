using Domain.Repositories;
using IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.Abstractions;

namespace IntegrationTests;

internal abstract class IntegrationTestBase : IAsyncDisposable, IDisposable
{
    protected readonly SqliteInMemDbHelper DbHelper;

    protected IntegrationTestBase()
    {
        DbHelper = new SqliteInMemDbHelper(services =>
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        });
    }
    
    public IServiceManager GetServiceManager() => DbHelper.ServiceProvider.GetRequiredService<IServiceManager>();
    
    public RepositoryDbContext GetDbContext() => DbHelper.ServiceProvider.GetRequiredService<RepositoryDbContext>();

    public async ValueTask DisposeAsync()
    {
        await DbHelper.DisposeAsync();
    }

    public void Dispose()
    {
        DbHelper.Dispose();
    }
}