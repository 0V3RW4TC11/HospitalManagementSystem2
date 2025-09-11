using Mapster;
using Services.Dtos.Admin;

namespace Presentation.Models.Admin
{
    public class AdminManageViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsLockedOut { get; set; }

        public AdminViewModel Admin { get; set; }

        public AdminManageViewModel()
        {
            
        }

        public AdminManageViewModel(
            AdminDto admin,
            string userName,
            bool isLockedOut)
        {
            Id = admin.Id;
            Username = userName;
            IsLockedOut = isLockedOut;
            Admin = admin.Adapt<AdminViewModel>();
        }

        public AdminDto Dto
        {
            get
            {
                var dto = Admin.Adapt<AdminDto>();
                dto.Id = Id;
                return dto;
            }
        }
    }
}
