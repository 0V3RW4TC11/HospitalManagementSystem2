using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Tests.Helpers;

public static class SpecializationTestHelper
{
    public const string TestSpec1Name = "TestSpecialization1";
    
    public const string TestSpec2Name = "TestSpecialization2";
    
    public static Specialization CreateTestSpec1() => new Specialization { Name = TestSpec1Name };
    
    public static Specialization CreateTestSpec2() => new Specialization { Name = TestSpec2Name };
    
    public static Specialization GetTestSpec1(ApplicationDbContext context)
        => context.Specializations.Single(s => s.Name == TestSpec1Name);
    
    public static Specialization GetTestSpec2(ApplicationDbContext context)
        => context.Specializations.Single(s => s.Name == TestSpec2Name);
}