using Commands.Admin.UpdateAdmin;
using FluentValidation;

namespace Validation.Admin
{
    // need query to check for existing admin with same email
    public class UpdateAdminValidator : AbstractValidator<UpdateAdminCommand>
    {
        public UpdateAdminValidator()
        {
            
        }
    }
}
