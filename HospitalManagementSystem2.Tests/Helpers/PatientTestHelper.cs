using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class PatientTestHelper
{
    public const BloodType BloodType = Models.Entities.BloodType.AbNegative;

    public static Patient CreatePatient() => new Patient
    {
        Title = PersonTestData.Title,
        FirstName = PersonTestData.FirstName,
        LastName = PersonTestData.LastName,
        Gender = PersonTestData.Gender,
        Address = PersonTestData.Address,
        Phone = PersonTestData.Phone,
        Email = PersonTestData.Email,
        DateOfBirth = PersonTestData.DateOfBirth,
        BloodType = BloodType
    };

    public static Patient CreateAndSeedPatient(ApplicationDbContext context)
    {
        var patient = CreatePatient();
        context.Patients.Add(patient);
        context.SaveChanges();

        context.Entry(patient).State = EntityState.Detached;
        return patient;
    }

    public static void AssertHasData(ApplicationDbContext context, Patient patient)
    {
        Assert.Contains(context.Patients, p =>
            p.Id == patient.Id &&
            p.Title == patient.Title &&
            p.FirstName == patient.FirstName &&
            p.LastName == patient.LastName &&
            p.Gender == patient.Gender &&
            p.Address == patient.Address &&
            p.Phone == patient.Phone &&
            p.Email == patient.Email &&
            p.DateOfBirth == patient.DateOfBirth);
    }
    
    public static void AssertHasNoData(ApplicationDbContext context, Patient patient)
    {
        Assert.DoesNotContain(context.Patients, p => p.Id == patient.Id);
    }
}