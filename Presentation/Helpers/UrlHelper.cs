using Microsoft.AspNetCore.Mvc;

namespace Presentation.Helpers
{
    internal static class UrlHelper
    {
        public static IActionResult RedirectOrDefaultAction(Controller controller, string? url, string defController, string defAction, object? routeValues = null)
        {
            if (!string.IsNullOrEmpty(url) && controller.Url.IsLocalUrl(url))
            {
                return controller.Redirect(url);
            }

            return controller.RedirectToAction(defAction, defController, routeValues);
        }
    }
}
