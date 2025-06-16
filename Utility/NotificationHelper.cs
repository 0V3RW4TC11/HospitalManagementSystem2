namespace HospitalManagementSystem2.Utility
{
    public class NotificationHelper
    {
        public static string MissingData(string nameOfMissingData, string nameOfType, string idOfObject)
        {
            return $"Missing/Corrupted {nameOfMissingData} data for {nameOfType} with Id {idOfObject}";
        }
    }
}
