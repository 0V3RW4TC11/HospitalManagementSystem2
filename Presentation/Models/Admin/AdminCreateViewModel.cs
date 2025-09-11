using Mapster;
using Services.Dtos.Admin;

namespace Presentation.Models.Admin
{
    public class AdminCreateViewModel
    {
        public AdminViewModel Admin { get; set; }

        public PasswordCreateViewModel PasswordCreate { get; set; }

        public AdminCreateDto Dto
        {
            get
            {
                var dto = Admin.Adapt<AdminCreateDto>();
                dto.Password = PasswordCreate.Password;
                return dto;
            }
        }
    }
}
