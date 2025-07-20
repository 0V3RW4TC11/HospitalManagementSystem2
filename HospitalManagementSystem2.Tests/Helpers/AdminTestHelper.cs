using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class AdminTestHelper
{
    public static Admin CreateAdmin() => new Admin
    {
        Title = PersonTestData.Title,
        FirstName = PersonTestData.FirstName,
        LastName = PersonTestData.LastName,
        Gender = PersonTestData.Gender,
        Address = PersonTestData.Address,
        Phone = PersonTestData.Phone,
        Email = PersonTestData.Email,
        DateOfBirth = PersonTestData.DateOfBirth
    };

    public static Admin CreateAndSeedAdmin(ApplicationDbContext context)
    {
        var admin = CreateAdmin();
        context.Admins.Add(admin);
        context.SaveChanges();

        context.Entry(admin).State = EntityState.Detached;
        return admin;
    }

    public static void AssertHasData(ApplicationDbContext context, Admin admin)
    {
        Assert.Contains(context.Admins, a =>
            a.Id == admin.Id &&
            a.Title == admin.Title &&
            a.FirstName == admin.FirstName &&
            a.LastName == admin.LastName &&
            a.Gender == admin.Gender &&
            a.Address == admin.Address &&
            a.Phone == admin.Phone &&
            a.Email == admin.Email &&
            a.DateOfBirth == admin.DateOfBirth);
    }

    public static void AssertHasNoData(ApplicationDbContext context, Admin admin)
    {
        Assert.DoesNotContain(context.Admins, a => a.Id == admin.Id);
    }
}