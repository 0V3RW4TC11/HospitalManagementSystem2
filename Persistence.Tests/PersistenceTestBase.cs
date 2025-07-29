using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence.Repositories;
using Persistence.Tests.Helpers;
using Services;
using Services.Abstractions;

namespace Persistence.Tests;

internal abstract class PersistenceTestBase
{
    private SqliteInMemDbHelper _dbHelper;
    private readonly Action<ServiceCollection>? _serviceConfig;

    protected PersistenceTestBase(Action<ServiceCollection>? serviceConfig)
    {
        _serviceConfig = serviceConfig;
    }

    [SetUp]
    public virtual void SetUp()
    {
        _dbHelper = new SqliteInMemDbHelper(_serviceConfig);
    }
    
    [TearDown]
    public virtual void TearDown()
    {
        _dbHelper.Dispose();
    }

    protected IServiceProvider GetServiceProvider() => _dbHelper.ServiceProvider;
    
    protected RepositoryDbContext GetDbContext() => _dbHelper.ServiceProvider.GetRequiredService<RepositoryDbContext>();
    
    protected IServiceManager GetServiceManager() => _dbHelper.ServiceProvider.GetRequiredService<IServiceManager>();
}