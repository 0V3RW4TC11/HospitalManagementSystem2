using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Helpers
{
    internal static class UrlHelper
    {
        public static IActionResult Redirect(Controller controller, string? url)
        {
            if (string.IsNullOrEmpty(url) || controller.Url.IsLocalUrl(url) is false)
            {
                return controller.RedirectToAction("Index", "Home");
            }

            return controller.Redirect(url);
        }
    }
}
