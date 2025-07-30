using DataTransfer.Patient;
using Domain.Constants;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace TestData;

public class PatientTestData
{
    public static PatientCreateDto CreateDto() => new()
    {
        FirstName = "TestPatientFirstName",
        LastName = "TestPatientLastName",
        Gender = "Male",
        Address = "123 Main St",
        Phone = "123-456-7890",
        Email = "testPatient@example.com",
        DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
        Password = "Password123!",
        BloodType = BloodType.AbPositive
    };

    public static async Task<PatientDto> SeedPatient(RepositoryDbContext context, UserManager<IdentityUser> userManager)
    {
        var patientCreateDto = CreateDto();
        var patient = patientCreateDto.Adapt<Patient>();
        
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        
        var identity = new IdentityUser {UserName = patientCreateDto.Email};
        await userManager.CreateAsync(identity, patientCreateDto.Password);
        await userManager.AddToRoleAsync(identity, AuthRoles.Patient);
        
        context.Accounts.Add(new Account {UserId = patient.Id, IdentityUserId = identity.Id});
        await context.SaveChangesAsync();
        
        return patient.Adapt<PatientDto>();
    }
}