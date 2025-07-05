namespace HospitalManagementSystem2.Models.Entities
{
    public class Admin : Staff
    {
        public Admin ShallowClone()
        {
            return (Admin)MemberwiseClone();
        }
    }
}
