using ViewModels.Identity;

namespace ViewModels.Shared
{
    public class CreateUserViewModel<T> where T : class
    {
        public T Data { get; set; }

        public CreatePasswordViewModel PasswordModel { get; set; }
    }
}
