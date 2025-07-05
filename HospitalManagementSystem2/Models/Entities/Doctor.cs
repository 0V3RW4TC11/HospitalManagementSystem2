namespace HospitalManagementSystem2.Models.Entities
{
    public class Doctor : Staff
    {
        public IEnumerable<Specialization> Specializations { get; set; }
    }
}
