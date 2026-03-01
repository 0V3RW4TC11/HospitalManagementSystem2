namespace Presentation.ViewModels.Attendance
{
    public class AttendanceResultsViewModel
    {
        public string ListName { get; set; }

        public string ResultName { get; set; }

        public X.PagedList.IPagedList<AttendanceResultViewModel> PagedResults { get; set; }

        public string ReturnRoute { get; set; }

        public Func<int, object> RouteValues { get; set; }
    }
}
