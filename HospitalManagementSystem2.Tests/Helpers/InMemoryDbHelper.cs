using HospitalManagementSystem2.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class InMemoryDbHelper
{
    public static ApplicationDbContext CreateInMemDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new ApplicationDbContext(options);
    }
}