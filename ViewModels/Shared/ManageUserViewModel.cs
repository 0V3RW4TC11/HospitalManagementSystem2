namespace ViewModels.Shared
{
    public class ManageUserViewModel<T> where T : class
    {
        public EditUserViewModel<T> Edit { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
