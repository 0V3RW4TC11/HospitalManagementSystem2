using System.Text.Json;
using ViewModels.Specialization;

namespace ViewModels.Doctor
{
    public class DoctorSpecializationsJson
    {
        private static JsonSerializerOptions JsonOptions => new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public string Json { get; set; } // View interacts with this

        public void SetJsonFromSpecializations(IEnumerable<SpecViewModel> models)
        {
            Json = JsonSerializer.Serialize(models, JsonOptions);
        }

        public IEnumerable<Guid> GetSpecializationIdsFromJson()
        {
            return JsonSerializer
                .Deserialize<IEnumerable<SpecViewModel>>(Json, JsonOptions)?
                .Select(s => s.Id)
                .ToHashSet() ?? throw new Exception("Failed to parse Json");
        }
    }
}
