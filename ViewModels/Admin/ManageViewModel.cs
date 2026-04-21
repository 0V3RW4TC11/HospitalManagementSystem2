namespace ViewModels.Admin
{
    public class ManageViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public bool IsLockedOut { get; set; }

        public DataViewModel Data { get; set; }
    }
}
