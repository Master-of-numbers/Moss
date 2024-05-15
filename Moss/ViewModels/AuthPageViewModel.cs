using System;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;

namespace Moss.ViewModels;

public class AuthPageViewModel : FirstStepPageViewModelBase
{
    public AuthPageViewModel()
    {
        LocalTransfer.WhiteCirclePos = 140;
        LocalTransfer.RedCirclePos = 0;
        _CurrentPage = Pages[1];

        LocalTransfer.AuthPageChangedEventAct += AuthPageChanged;
        
        var canNavReg = this.WhenAnyValue(x => x.CurrentPage.CanNavigateRegistration);
        var canNavLog = this.WhenAnyValue(x => x.CurrentPage.CanNavigateLogin);
        
        NavigateLoginCommand = ReactiveCommand.Create(NavigateLogin, canNavLog);
        NavigateRegistrationCommand = ReactiveCommand.Create(NavigateRegistration, canNavReg);
    }

    private readonly AuthPageViewModelBase[] Pages =
    {
        new AuthRegistrationPageViewModel(),
        new AuthLoginPageViewModel()
    };

    private AuthPageViewModelBase _CurrentPage;
    public AuthPageViewModelBase CurrentPage
    {
        get { return _CurrentPage; }
        private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
    }

    public int RedCirclePos
    {
        get => LocalTransfer.RedCirclePos;
        set => LocalTransfer.RedCirclePos = value;
    }
    public int WhiteCirclePos
    {
        get => LocalTransfer.WhiteCirclePos;
        set => LocalTransfer.WhiteCirclePos = value;
    }

    private void NavigateLogin()
    {
        LocalTransfer.RedCirclePos = 0;
        LocalTransfer.WhiteCirclePos = 140;
        LocalTransfer.AuthPageChangedEvent();
        CurrentPage = Pages[1];
    }
    private void NavigateRegistration()
    {
        LocalTransfer.RedCirclePos = 140;
        LocalTransfer.WhiteCirclePos = 0;
        LocalTransfer.AuthPageChangedEvent();
        CurrentPage = Pages[0];
    }
    private void AuthPageChanged()
    {
        RedCirclePos = LocalTransfer.RedCirclePos;
        RedCirclePos = LocalTransfer.WhiteCirclePos;
    }
    public ICommand NavigateRegistrationCommand { get; }
    public ICommand NavigateLoginCommand { get; }
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
    private bool _CanNavigateUserPage = true;
    public override bool CanNavigateUserPage
    {
        get => _CanNavigateUserPage;
        set => this.RaiseAndSetIfChanged(ref _CanNavigateUserPage, value);
    }
}