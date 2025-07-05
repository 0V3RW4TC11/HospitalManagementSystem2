using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities
{
    public class Patient : Account
    {
        public BloodType BloodType { get; set; }

        public static Expression<Func<Patient, bool>> Matches(Patient patient)
        {
            return x => x.Email.ToLower() == patient.Email.ToLower() ||
                        x.FirstName.ToLower() == patient.FirstName.ToLower() &&
                        x.LastName.ToLower() == patient.LastName.ToLower() &&
                        x.DateOfBirth == patient.DateOfBirth;
        }
    }

    public enum BloodType
    {
        APositive,
        ANegative,
        BPositive,
        BNegative,
        ABPositive,
        ABNegative,
        OPositive,
        ONegative
    };
}
