namespace ViewModels.User
{
    public class EditViewModel<T> where T : class
    {
        public T Data { get; set; }
        
        public Guid Id { get; set; }

        public string UserName { get; set; }
    }
}
