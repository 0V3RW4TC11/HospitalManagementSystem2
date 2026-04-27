using ViewModels.User;

namespace ViewModels.Doctor
{
    public class ManageDoctorViewModel : ManageViewModel<DoctorDataViewModel>
    {
        public IEnumerable<string> SpecializationNames { get; set; } = new List<string>();
    }
}
