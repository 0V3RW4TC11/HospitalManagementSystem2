using Dtos.Doctor;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.Doctor
{
    internal class DoctorCorrectnessValidator : AbstractValidator<DoctorDto>
    {
        public DoctorCorrectnessValidator()
        {
            RuleFor(d => d).NotNull().WithMessage("Doctor details are required.");
            RuleFor(d => d.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(d => d.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(d => d.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}
