using DataTransfer.Doctor;
using DataTransfer.Patient;

namespace Seeding
{
    internal static class Data
    {
        public static string[] Specializations = ["Cardiology", "Neurology", "Pediatrics"];

        public static DoctorCreateDto DoctorA() => new()
        {
            FirstName = "Doctor",
            LastName = "A",
            Gender = "Male",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "doctor.a@example.com",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!",
            SpecializationIds = new HashSet<Guid>()
        };

        public static DoctorCreateDto DoctorB() => new()
        {
            FirstName = "Doctor",
            LastName = "B",
            Gender = "Female",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "doctor.b@example.com",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!",
            SpecializationIds = new HashSet<Guid>()
        };

        public static DoctorCreateDto DoctorC() => new()
        {
            FirstName = "Doctor",
            LastName = "C",
            Gender = "Male",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "doctor.c@example.com",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!",
            SpecializationIds = new HashSet<Guid>()
        };

        public static DoctorCreateDto[] Doctors() => [DoctorA(), DoctorB(), DoctorC()];

        public static PatientCreateDto PatientA() => new()
        {
            Title = "Mr",
            FirstName = "Patient",
            LastName = "A",
            Gender = "Male",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "patient.a@example.com",
            BloodType = Constants.BloodType.APositive,
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!"
        };

        public static PatientCreateDto PatientB() => new()
        {
            Title = "Mrs",
            FirstName = "Patient",
            LastName = "B",
            Gender = "Female",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "patient.b@example.com",
            BloodType = Constants.BloodType.AbNegative,
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!"
        };

        public static PatientCreateDto PatientC() => new()
        {
            Title = "Mr",
            FirstName = "Patient",
            LastName = "C",
            Gender = "Male",
            Address = "Example Address",
            Phone = "00-0-0000-0000",
            Email = "patient.c@example.com",
            BloodType = Constants.BloodType.ONegative,
            DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
            Password = "Pass123!"
        };

        public static PatientCreateDto[] Patients() => [PatientA(), PatientB(), PatientC()];
    }
}
