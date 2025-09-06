using System.ComponentModel.DataAnnotations;

namespace Constants;

public enum BloodType
{
    [Display(Name = "A+")]
    APositive,
    [Display(Name = "A-")]
    ANegative,
    [Display(Name = "B+")]
    BPositive,
    [Display(Name = "B-")]
    BNegative,
    [Display(Name = "AB+")]
    AbPositive,
    [Display(Name = "AB-")]
    AbNegative,
    [Display(Name = "O+")]
    OPositive,
    [Display(Name = "O-")]
    ONegative
}
