namespace Presentation.ViewModels.Admin
{
    public class ManageAdminViewModel : AdminViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
