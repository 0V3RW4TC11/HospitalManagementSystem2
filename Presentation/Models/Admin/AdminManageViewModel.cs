namespace Presentation.Models.Admin
{
    public class AdminManageViewModel : AdminBaseViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
