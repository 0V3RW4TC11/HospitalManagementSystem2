using Microsoft.AspNetCore.Mvc;

namespace Presentation.Helpers
{
    internal static class ControllerNameExtensions
    {
        public static string GetControllerName<T>() where T : Controller
        {
            return typeof(T).Name.Replace("Controller", string.Empty);
        }
    }
}
