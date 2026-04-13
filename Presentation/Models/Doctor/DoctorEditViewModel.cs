using Mapster;
using Services.Dtos.Doctor;
using Services.Dtos.Specialization;
using System.Text.Json;

namespace Presentation.Models.Doctor
{
    public class DoctorEditViewModel
    {
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public DoctorViewModel Doctor { get; set; }

        public string JsonSpecializations { get; set; }

        public HashSet<Guid> SelectedSpecs => JsonSerializer.Deserialize<IEnumerable<SpecializationDto>>(JsonSpecializations, _jsonOptions)?
            .Select(s => s.Id)
            .ToHashSet() ?? throw new Exception("Failed to parse " + nameof(JsonSpecializations));

        public DoctorEditViewModel() 
        {

        }

        public DoctorEditViewModel(DoctorDto doctor, IEnumerable<SpecializationDto> allSpecs)
        {
            Doctor = doctor.Adapt<DoctorViewModel>();

            var specs = allSpecs
                .Where(s => doctor.SpecializationIds.Contains(s.Id))
                .ToArray();

            JsonSpecializations = 
                JsonSerializer.Serialize(specs, _jsonOptions);
        }
    }
}
