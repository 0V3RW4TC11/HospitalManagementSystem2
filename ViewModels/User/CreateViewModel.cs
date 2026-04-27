using ViewModels.Identity;

namespace ViewModels.User
{
    public class CreateViewModel<T> where T : class
    {
        public T Data { get; set; }

        public CreatePasswordViewModel PasswordModel { get; set; }
    }
}
