using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.TestData;

public static class PatientTestData
{
    public const string Title = "TestTitle";
    public const string FirstName = "TestFirstName";
    public const string LastName = "TestLastName";
    public const string Gender = "TestGender";
    public const string Address = "TestAddress";
    public const string Phone = "TestPhoneNumber";
    public const string Email = "TestEmail";
    public const string TestPassword = "TestPassword123!";
    public const BloodType BloodType = Models.Entities.BloodType.AbNegative;
    public static readonly DateOnly DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);

    public static Patient CreatePatient() => new Patient
    {
        Title = Title,
        FirstName = FirstName,
        LastName = LastName,
        Gender = Gender,
        Address = Address,
        Phone = Phone,
        Email = Email,
        DateOfBirth = DateOfBirth,
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
}