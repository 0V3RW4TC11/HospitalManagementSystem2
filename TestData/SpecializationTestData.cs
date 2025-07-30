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

    public static async Task<IEnumerable<SpecializationDto>> SeedSpecializationsAsync(
        RepositoryDbContext context,
        params SpecializationCreateDto[] createDtos)
    {
        context.Specializations.AddRange(createDtos.Adapt<IEnumerable<Specialization>>());
        await context.SaveChangesAsync();
        
        return await context.Specializations.Select(s => s.Adapt<SpecializationDto>()).ToArrayAsync();
    }
}