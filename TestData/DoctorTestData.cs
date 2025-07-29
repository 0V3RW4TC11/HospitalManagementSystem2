using DataTransfer.Admin;
using DataTransfer.Doctor;
using Domain.Constants;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace TestData;

public static class DoctorTestData
{
    private const string FirstName = "TestDoctorFirstName";
    private const string LastName = "TestDoctorLastName";
    public static readonly string ExpectedUsername =
        $"{FirstName.ToLower()}.{LastName.ToLower()}@{DomainNames.Organization}";

    public static DoctorCreateDto CreateDto(ISet<Guid> specializationIds) => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Gender = "Male",
        Address = "123 Main St",
        Phone = "123-456-7890",
        Email = "testDoctor@example.com",
        DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
        Password = "Password123!",
        SpecializationIds = specializationIds
    };
    
    public static async Task<DoctorDto> SeedDoctor(
        RepositoryDbContext context, 
        UserManager<IdentityUser> userManager,
        ISet<Guid> specializationIds)
    {
        var doctorCreateDto = CreateDto(specializationIds);
        var doctor = doctorCreateDto.Adapt<Doctor>();
        
        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();
        
        await context.DoctorSpecializations.AddRangeAsync(specializationIds.Select(sId => 
            new DoctorSpecialization {DoctorId = doctor.Id, SpecializationId = sId}));
        await context.SaveChangesAsync();
        
        var identity = new IdentityUser {UserName = ExpectedUsername};
        await userManager.CreateAsync(identity, doctorCreateDto.Password);
        await userManager.AddToRoleAsync(identity, AuthRoles.Doctor);
        
        context.Accounts.Add(new Account {UserId = doctor.Id, IdentityUserId = identity.Id});
        await context.SaveChangesAsync();
        
        var seededDoctorDto = doctor.Adapt<DoctorDto>();
        seededDoctorDto.SpecializationIds = specializationIds;
        
        return seededDoctorDto;
    }
}