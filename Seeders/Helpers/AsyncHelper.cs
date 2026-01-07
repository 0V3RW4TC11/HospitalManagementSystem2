namespace Seeders.Helpers
{
    internal static class AsyncHelper
    {
        public static Task InvokeAsync(Func<Task> func) => func();
    }
}
