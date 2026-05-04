namespace ViewModels.Shared
{
    public class EditUserViewModel<T> where T : class
    {
        public T Data { get; set; }

        public Guid Id { get; set; }

        public string UserName { get; set; }
    }
}
