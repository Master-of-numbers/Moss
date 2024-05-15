namespace Moss.ViewModels;

public abstract class AuthPageViewModelBase : ViewModelBase
{
    public abstract bool CanNavigateLogin { get; set; }
    public abstract bool CanNavigateRegistration { get; set; }
    public abstract bool CanNavigateUserPage { get; set; }
}