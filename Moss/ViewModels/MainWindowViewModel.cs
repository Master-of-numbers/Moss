using System;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
namespace Moss.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Variables

    private readonly FirstStepPageViewModelBase[] Pages =
        {
            new AuthPageViewModel(),
            new UserPageViewModel(),
            new SettingsPageViewModel()
        };
    private FirstStepPageViewModelBase _CurrentPage;
    
    #endregion
    public MainWindowViewModel()
    {
        LocalTransfer.UserID = 1;
        // init database
        DataManager.InitializeDatabase();
        _CurrentPage = Pages[0];
        //_CurrentPage = Pages[LocalTransfer.PageFromList];
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateUserPage);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigateAuthPage);
        var canNavSett = this.WhenAnyValue(x => x.CurrentPage.CanNavigateSettings);

        LocalTransfer.PageFromListChangedEvent += OnStaticEventChanged;

        NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious);
        NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings);
    }

    #region Interfaces
    public FirstStepPageViewModelBase CurrentPage
    {
        get => _CurrentPage;
        set => this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }
    public ICommand NavigateNextCommand { get; }
    public ICommand NavigatePreviousCommand { get; }
    public ICommand NavigateSettingsCommand { get; }

    #endregion

    #region Methods

    private void OnStaticEventChanged()
    {
        // Handle the event when the value in StaticClass is changed
        switch (LocalTransfer.PageFromList)
        {
            case 2:
                NavigateSettings();
                break;
            case 1:
                NavigateUserPage();
                break;
            case 0:
                NavigateAuthPage();
                break;
        }
        // For example, you can invoke a method when the static event is raised
    }
    public void NavigateSettings()
    {
        CurrentPage = Pages[2];
    }
    public void NavigateAuthPage()
    {
        CurrentPage = Pages[0];
    }
    public void NavigateUserPage()
    {
        CurrentPage = Pages[1];
    }
    private void NavigateNext()
    {
        var index = Pages.IndexOf(CurrentPage) + 1;
        LocalTransfer.PageFromList = index;
        CurrentPage = Pages[LocalTransfer.PageFromList];
    }
    private void NavigatePrevious()
    {
        var index = Pages.IndexOf(CurrentPage) - 1;
        CurrentPage = Pages[index];
    }

    #endregion
}