using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class DoctorTestHelper
{
    public static Doctor CreateDoctor(IEnumerable<Specialization>? specializations = null)
    {
        var doctor = new Doctor
        {
            FirstName = PersonTestData.FirstName,
            LastName = PersonTestData.LastName,
            Gender = PersonTestData.Gender,
            Address = PersonTestData.Address,
            Phone = PersonTestData.Phone,
            Email = PersonTestData.Email,
            DateOfBirth = PersonTestData.DateOfBirth
        };

        doctor.Specializations = specializations ?? [];

        return doctor;
    }

    public static Doctor CreateAndSeedDoctor(ApplicationDbContext context,
        IEnumerable<Specialization>? specializations = null)
    {
        // Create Doctor with specs if any
        var doctor = CreateDoctor(specializations);

        // Add Doctor to database
        context.Doctors.Add(doctor);
        context.SaveChanges();

        // If specs, add DoctorSpecs to database
        if (specializations != null)
        {
            foreach (var spec in specializations)
            {
                context.DoctorSpecializations.Add(new DoctorSpecialization
                {
                    DoctorId = doctor.Id,
                    SpecializationId = spec.Id
                });
            }
            context.SaveChanges();
        }

        // Return Doctor
        context.Entry(doctor).State = EntityState.Detached;
        return doctor;
    }

    public static void AssertHasData(ApplicationDbContext context, Doctor doctor)
    {
        Assert.Contains(context.Doctors, d => 
            d.Id == doctor.Id &&
            d.FirstName == doctor.FirstName &&
            d.LastName == doctor.LastName &&
            d.Gender == doctor.Gender &&
            d.Address == doctor.Address &&
            d.Phone == doctor.Phone &&
            d.Email == doctor.Email &&
            d.DateOfBirth == doctor.DateOfBirth);

        Assert.True(context.DoctorSpecializations.Where(ds => ds.DoctorId == doctor.Id)
            .All(ds => doctor.Specializations.Select(s => s.Id).Contains(ds.SpecializationId)));
    }

    public static void AssertHasNoData(ApplicationDbContext context, Doctor doctor)
    {
        Assert.DoesNotContain(context.Doctors, d => d.Id == doctor.Id);
        Assert.DoesNotContain(context.DoctorSpecializations, ds => ds.DoctorId == doctor.Id);
    }
}