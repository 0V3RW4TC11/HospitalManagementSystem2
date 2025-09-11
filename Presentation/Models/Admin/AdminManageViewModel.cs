namespace Presentation.Models.Admin
{
    public class AdminManageViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
