namespace Moss.ViewModels;

public abstract class FirstStepPageViewModelBase : ViewModelBase
{
    public abstract bool CanNavigateAuthPage { get; set; }
    public abstract bool CanNavigateUserPage { get; set; }
    public abstract bool CanNavigateSettings { get; set; }
}