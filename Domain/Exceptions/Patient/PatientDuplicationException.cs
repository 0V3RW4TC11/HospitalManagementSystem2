namespace Domain.Exceptions.Patient
{
    public class PatientDuplicationException : Exception
    {
        public PatientDuplicationException() : base("Email is in use by another " + Constants.AuthRoles.Patient)
        {
        }
    }
}
