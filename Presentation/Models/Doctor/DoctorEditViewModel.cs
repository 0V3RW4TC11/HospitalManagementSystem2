using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;

namespace Presentation.Models.Doctor
{
    public class DoctorEditViewModel
    {
        public DoctorViewModel Doctor { get; set; }

        public DoctorEditSpecViewModel[] Specializations { get; set; }

        public HashSet<Guid> SelectedSpecs => Specializations
            .Where(s => s.IsSelected)
            .Select(s => s.Id)
            .ToHashSet();

        public DoctorEditViewModel() 
        {

        }

        public DoctorEditViewModel(IEnumerable<SpecializationDto> allSpecs)
        {
            Specializations = allSpecs
                .Select(s => new DoctorEditSpecViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = false
                })
                .ToArray();
        }

        public DoctorEditViewModel(DoctorDto doctor, IEnumerable<SpecializationDto> allSpecs)
        {
            Doctor = doctor.Adapt<DoctorViewModel>();

            Specializations = allSpecs
                .Select(s => new DoctorEditSpecViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = doctor.SpecializationIds.Contains(s.Id)
                })
                .ToArray();
        }
    }
}
