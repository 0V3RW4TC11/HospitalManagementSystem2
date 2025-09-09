namespace Presentation.Models.Admin
{
    public class AdminManageViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }

        public AdminDetailsViewModel DetailsViewModel { get; set; }
    }
}
