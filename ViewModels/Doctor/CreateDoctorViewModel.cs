using ViewModels.User;

namespace ViewModels.Doctor
{
    public class CreateDoctorViewModel : CreateViewModel<DoctorDataViewModel>
    {
        public DoctorSpecializationsJson SpecializationsJson { get; set; }
    }
}
