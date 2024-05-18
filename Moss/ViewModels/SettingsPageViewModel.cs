using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Avalonia.Threading;
using System;

namespace Moss.ViewModels;

public class SettingsPageViewModel : FirstStepPageViewModelBase
{
    #region Variables
    
    private string? _NewUserName;
    private bool _CanChangeUserName;
    private string? _NewPassword;
    private string? _notificationText;
    private bool _notificationMessageIsVisible;
    private string? _userToken;
    private bool _canSetToken;
    private bool _CanChangePassword;

    private bool _CanDeleteUser;

    #endregion
    public SettingsPageViewModel()
    {
        DataManager.InitializeDatabase();

        UserToken = LocalTransfer.UserToken;
        
        var canNavUserPage = this.WhenAnyValue(x => x.CanNavigateUserPage);
        
        this.WhenAnyValue(x => x.NewPassword).Subscribe(_ => UpdateCanChangePassword());
        this.WhenAnyValue(x => x.NewUserName).Subscribe(_ => UpdateCanChangeUserName());
        this.WhenAnyValue(x => x.UserToken).Subscribe(_ => UpdateCanSetToken());
        
        var canChangePassword = this.WhenAnyValue(x => x.CanChangePassword);
        var canChangeUserName = this.WhenAnyValue(x => x.CanChangeUserName);

        var canSetToken = this.WhenAnyValue(x => x.CanSetToken);
        var canDeleteUser = this.WhenAnyValue(x => x.CanDeleteUser);
        
        NavigateUserPageCommand = ReactiveCommand.Create(NavigateUserPage, canNavUserPage);
        GetAPITokenCommand = ReactiveCommand.Create(OpenGetTokenInBrowser);
        GoToAqicnCommand = ReactiveCommand.Create(OpenAqicnInBrowser);

        ActivateDeleteButtonCommand = ReactiveCommand.Create(ActivateDeleteButton);
        DeactivateDeleteButtonCommand = ReactiveCommand.Create(DeactivateDeleteButton);

        SetUserTokenCommand = ReactiveCommand.Create(SetUserToken, canSetToken);
        DeleteUserCommand = ReactiveCommand.Create(DeleteUser, canDeleteUser);
        
        ChangePasswordCommand = ReactiveCommand.Create(SetNewPassword, canChangePassword);
        ChangeUserNameCommand = ReactiveCommand.Create(SetNewUserName, canChangeUserName);
    }

    #region Properties
    
    public bool CanSetToken
    {
        get => _canSetToken;
        set => this.RaiseAndSetIfChanged(ref _canSetToken, value);
    }
    public string? UserToken
    {
        get => _userToken;
        set => this.RaiseAndSetIfChanged(ref _userToken, value);
    }
    public string? NotificationText
    {
        get => _notificationText;
        set => this.RaiseAndSetIfChanged(ref _notificationText, value);
    }
    public bool NotificationIsVisible
    {
        get => _notificationMessageIsVisible;
        set => this.RaiseAndSetIfChanged(ref _notificationMessageIsVisible, value);
    }
    public string? NewPassword
    {
        get => _NewPassword;
        set => this.RaiseAndSetIfChanged(ref _NewPassword, value);
    }
    public string? NewUserName
    {
        get => _NewUserName;
        set => this.RaiseAndSetIfChanged(ref _NewUserName, value);
    }
    private bool CanChangePassword
    {
        get => _CanChangePassword;
        set => this.RaiseAndSetIfChanged(ref _CanChangePassword, value);
    }
    private bool CanChangeUserName
    {
        get => _CanChangeUserName;
        set => this.RaiseAndSetIfChanged(ref _CanChangeUserName, value);
    }
    private bool CanDeleteUser
    {
        get => _CanDeleteUser;
        set => this.RaiseAndSetIfChanged(ref _CanDeleteUser, value);
    }
    public override bool CanNavigateSettings
    {
        get => false;
        set => CanNavigateSettings = false;
    }
    public override bool CanNavigateAuthPage
    {
        get => true;
        set => CanNavigateAuthPage = true;
    }
    public override bool CanNavigateUserPage
    {
        get => true;
        set => CanNavigateSettings = true;
    }

    #endregion

    #region Interfaces
    public ICommand NavigateUserPageCommand { get; set; }
    public ICommand GetAPITokenCommand { get; set; }
    public ICommand GoToAqicnCommand { get; set; }
    public ICommand ChangePasswordCommand { get; set; }
    public ICommand ChangeUserNameCommand { get; set; }
    public ICommand ActivateDeleteButtonCommand { get; set; }
    public ICommand DeactivateDeleteButtonCommand { get; set; }
    public ICommand DeleteUserCommand { get; set; }
    public ICommand SetUserTokenCommand { get; set; }

    #endregion

    #region Methods

    private void ActivateDeleteButton()
        {
            CanDeleteUser = true;
        }
    private void DeactivateDeleteButton()
        {
            CanDeleteUser = false;
        }
    private void DeleteUser()
        {
            DataManager.DeleteUser(LocalTransfer.UserID);
            NavigateAuthPage();
            DeactivateDeleteButton();
        }
    private void SetUserToken()
        {
            DataManager.SetUserToken(LocalTransfer.UserID, UserToken);
            LocalTransfer.UserToken = UserToken;
            Notification(3000,"Token Setted!");
            UserToken = null;
        }
    private async void Notification(int ms_time, string text)
        {
            NotificationText = text;
            NotificationIsVisible = true;
            await Task.Delay(ms_time);
            NotificationIsVisible = false;
        }
    private void OpenGetTokenInBrowser()
        {
            Process.Start(new ProcessStartInfo("https://aqicn.org/data-platform/token/") { UseShellExecute = true });
        }
    private void OpenAqicnInBrowser()
        {
            Process.Start(new ProcessStartInfo("https://aqicn.org/") { UseShellExecute = true });
        }
    private void SetNewPassword()
        {
            DataManager.ChangePassword(LocalTransfer.UserID, NewPassword);
            Notification(4000,"Password Changed!");
            NewPassword = null;
        }
    private void SetNewUserName()
        {
            DataManager.ChangeUserName(LocalTransfer.UserID, NewUserName);
            Notification(4000, "UserName Changed!");
            NewUserName = null;
        }
    private void UpdateCanChangePassword()
        {
            CanChangePassword = !string.IsNullOrEmpty(NewPassword) && NewPassword.Length >= 8;
        }
    private void UpdateCanChangeUserName()
        {
            CanChangeUserName = !string.IsNullOrEmpty(NewUserName) && !DataManager.UsernameExists(NewUserName);
        }
    private void UpdateCanSetToken()
        {
            CanSetToken = !string.IsNullOrEmpty(UserToken);
        }
    private void NavigateUserPage()
        {
            LocalTransfer.PageFromList = 1;
            LocalTransfer.RaiseStaticEvent();
        }
    private void NavigateAuthPage()
        {
            LocalTransfer.PageFromList = 0;
            LocalTransfer.RaiseStaticEvent();
        }

    #endregion
}