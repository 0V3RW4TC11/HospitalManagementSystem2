using ViewModels.User;

namespace ViewModels.Doctor
{
    public class ProfileDoctorViewModel : EditViewModel<DoctorDataViewModel>
    {
        public IEnumerable<string> SpecializationNames { get; set; }
    }
}
