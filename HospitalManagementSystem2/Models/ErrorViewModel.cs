<<<<<<<< HEAD:Presentation/Models/ErrorViewModel.cs
namespace Presentation.Models;
========
namespace HospitalManagementSystem2.Models;
>>>>>>>> origin/main:HospitalManagementSystem2/Models/ErrorViewModel.cs

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}