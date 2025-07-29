using DataTransfer.Specialization;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace TestData;

public static class SpecializationTestData
{
    public static SpecializationCreateDto CreateSpec1Dto() => new()
    {
        Name = "Test Specialization 1"
    };
    
    public static SpecializationCreateDto CreateSpec2Dto() => new()
    {
        Name = "Test Specialization 2"
    };
    
    public static SpecializationCreateDto CreateSpec3Dto() => new()
    {
        Name = "Test Specialization 3"
    };

    public static async Task<IEnumerable<Guid>> SeedSpecializationsAsync(RepositoryDbContext context)
    {
        var spec1 = CreateSpec1Dto().Adapt<Specialization>();
        var spec2 = CreateSpec2Dto().Adapt<Specialization>();
        
        context.Specializations.AddRange(spec1, spec2);
        await context.SaveChangesAsync();
        
        return await context.Specializations.Select(s => s.Id).ToArrayAsync();
    }
}