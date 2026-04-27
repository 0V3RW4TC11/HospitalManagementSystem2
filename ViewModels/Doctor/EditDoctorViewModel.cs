using ViewModels.User;

namespace ViewModels.Doctor
{
    public class EditDoctorViewModel : EditViewModel<DoctorDataViewModel>
    {
        public DoctorSpecializationsJson SpecializationsJson { get; set; }
    }
}
