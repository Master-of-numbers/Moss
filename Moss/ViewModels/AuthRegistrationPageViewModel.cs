using System;
using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace Moss.ViewModels;

public class AuthRegistrationPageViewModel : AuthPageViewModelBase
{
    public AuthRegistrationPageViewModel()
    {
        this.WhenAnyValue(x => x.UserName, x => x.Password)
            .Subscribe(_ => UpdateCanNavigateUserPage());
        var canNavUserPage = this.WhenAnyValue(x => x.CanNavigateUserPage);
        RegNavigateUserPageCommand = ReactiveCommand.Create(NavigateUserPage, canNavUserPage);
    }
    public string? _UserName, _Password;
    public string? UserName
    {
        get => _UserName;
        set => this.RaiseAndSetIfChanged(ref _UserName, value);
    }

    public string? Password
    {
        get => _Password;
        set => this.RaiseAndSetIfChanged(ref _Password, value);
    }

    public override bool CanNavigateLogin
    {
        get => true;
        set => CanNavigateLogin = value;
    }
    public override bool CanNavigateRegistration
    {
        get => false;
        set => CanNavigateRegistration = value;
    }

    private bool _CanNavigateUserPage;
    public override bool CanNavigateUserPage
    {
        get => _CanNavigateUserPage;
        set => this.RaiseAndSetIfChanged(ref _CanNavigateUserPage, value);
    }
    public void NavigateUserPage()
    {
        if (!DataManager.InsertUser(_UserName, _Password))
        {
            Debug.WriteLine("NavigateUserPage error");
        }
        else
        {
            (_UserName, _Password) = (null, null);
            Debug.Write(DataManager.DisplayUsers());
            LocalTransfer.PageFromList = 1;
            LocalTransfer.RaiseStaticEvent();
        }
    }
    public ICommand RegNavigateUserPageCommand { get; }
    private void UpdateCanNavigateUserPage()
    {
        CanNavigateUserPage=
            !string.IsNullOrEmpty(_UserName)
            && !string.IsNullOrEmpty(_Password)
            && _Password.Length >= 8;
    }
}