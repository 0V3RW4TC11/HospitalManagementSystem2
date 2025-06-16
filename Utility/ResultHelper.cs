using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Utility
{
    public class ResultHelper
    {
        public static void CheckIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }
}
