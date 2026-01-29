namespace Domain.Exceptions.Specialization
{
    public class SpecializationNameInUseException : Exception
    {
        public SpecializationNameInUseException(string name) : base("Specialization name already used: " + name)
        {
        }
    }
}
