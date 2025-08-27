using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Helpers
{
    internal static class ModelHelper
    {
        public static void AddErrorsToModel(ModelStateDictionary dictionary, IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                dictionary.AddModelError(string.Empty, error);
            }
        }

        public static IEnumerable<string> AdaptIdentityErrorsToStrings(IEnumerable<IdentityError> errors)
        {
            return errors.Select(e => e.Description);
        }
    }
}
