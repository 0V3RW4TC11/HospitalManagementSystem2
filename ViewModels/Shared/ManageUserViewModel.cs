namespace ViewModels.Shared
{
    public class ManageUserViewModel<T> where T : class
    {
        public T Data { get; set; }

        public Guid Id { get; set; }

        public bool IsLockedOut { get; set; }

        public string UserName { get; set; }
    }
}
