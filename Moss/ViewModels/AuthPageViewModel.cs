using System;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;

namespace Moss.ViewModels;

public class AuthPageViewModel : FirstStepPageViewModelBase
{
    #region Variables

    private bool _CanNavigateUserPage = true;
    private readonly AuthPageViewModelBase[] Pages =
    {
        new AuthRegistrationPageViewModel(),
        new AuthLoginPageViewModel()
    };
    private AuthPageViewModelBase _CurrentPage;

    #endregion

    public AuthPageViewModel()
    {
        _CurrentPage = Pages[1];
        
        var canNavReg = this.WhenAnyValue(x => x.CurrentPage.CanNavigateRegistration);
        var canNavLog = this.WhenAnyValue(x => x.CurrentPage.CanNavigateLogin);
        
        NavigateLoginCommand = ReactiveCommand.Create(NavigateLogin, canNavLog);
        NavigateRegistrationCommand = ReactiveCommand.Create(NavigateRegistration, canNavReg);
    }

    #region Properties
    public override bool CanNavigateSettings
    {
        get => false;
        set => CanNavigateSettings = false;
    }
    public override bool CanNavigateAuthPage
    {
        get => false;
        set => CanNavigateAuthPage = false;
    }
    public override bool CanNavigateUserPage
    {
        get => _CanNavigateUserPage;
        set => this.RaiseAndSetIfChanged(ref _CanNavigateUserPage, value);
    }
    public AuthPageViewModelBase CurrentPage
    {
        get => _CurrentPage;
        private set => this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }

    #endregion

    #region Interfaces
    public ICommand NavigateRegistrationCommand { get; }
    public ICommand NavigateLoginCommand { get; }

    #endregion

    #region Methods
    private void NavigateLogin()
    {
        CurrentPage = Pages[1];
    }
    private void NavigateRegistration()
    {
        CurrentPage = Pages[0];
    }
    
    #endregion
    
}