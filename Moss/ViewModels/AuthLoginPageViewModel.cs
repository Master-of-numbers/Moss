using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ReactiveUI;

namespace Moss.ViewModels;

public class AuthLoginPageViewModel : AuthPageViewModelBase
{
    #region Variables

    public string? _UserName, _Password, _UsersInBase;
    private bool _CanNavigateUserPage = true;

    #endregion
    public AuthLoginPageViewModel()
    {
        UserName = "";
        this.WhenAnyValue(x => x.UserName, x => x.Password)
            .Subscribe(_ => UpdateCanNavigateUserPage());
        var canNavUserPage = this.WhenAnyValue(x => x.CanNavigateUserPage);
        LoginNavigateSettings = ReactiveCommand.Create(NavigateUserPage, canNavUserPage);
        ShowUsersInBaseCommand = ReactiveCommand.Create(ShowUsersInBase, canNavUserPage);
    }

    #region Properties

    public string? UsersInBase
    {
        get => _UsersInBase;
        set => this.RaiseAndSetIfChanged(ref _UsersInBase, value);
    }

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
        get => false;
        set => CanNavigateLogin = value;
    }

    public override bool CanNavigateRegistration
    {
        get => true;
        set => CanNavigateRegistration = value;
    }

    public override bool CanNavigateUserPage
    {
        get => _CanNavigateUserPage;
        set => this.RaiseAndSetIfChanged(ref _CanNavigateUserPage, value);
    }

    #endregion

    #region Interfaces

    public ICommand ShowUsersInBaseCommand { get; }

    public ICommand LoginNavigateSettings { get; }

    #endregion

    #region Methods

    private void ShowUsersInBase()
    {
        UsersInBase = DataManager.DisplayUsers();
    }

    private void UpdateCanNavigateUserPage()
    {
        CanNavigateUserPage =
            !string.IsNullOrEmpty(_UserName)
            && !string.IsNullOrEmpty(_Password);
    }

    private void NavigateUserPage()
    {
        if (DataManager.UsernameExists(_UserName) && DataManager.PasswordIsValid(_UserName, _Password))
        {
            (_UserName, _Password) = (null, null);
            LocalTransfer.PageFromList = 1;
            LocalTransfer.RaiseStaticEvent();
        }
    }

    #endregion
}