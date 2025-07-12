namespace HospitalManagementSystem2.Helpers;

public class NotificationHelper
{
    public static string MissingData(string nameOfMissingData, string nameOfType, string idOfObject)
    {
        return $"Missing {nameOfMissingData} data for {nameOfType} with Id {idOfObject}";
    }
}