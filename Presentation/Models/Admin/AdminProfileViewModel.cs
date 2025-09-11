using Mapster;
using Services.Dtos.Admin;

namespace Presentation.Models.Admin
{
    public class AdminProfileViewModel
    {
        public AdminViewModel Admin { get; set; }

        public string UserName { get; set; }

        public AdminProfileViewModel()
        {
            
        }

        public AdminProfileViewModel(AdminDto admin, string userName)
        {
            Admin = admin.Adapt<AdminViewModel>();
            UserName = userName;
        }

        public AdminDto Dto(Guid id)
        {
            var dto = Admin.Adapt<AdminDto>();
            dto.Id = id;
            return dto;
        }
    }
}
