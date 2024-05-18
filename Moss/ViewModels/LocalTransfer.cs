using System;
using ReactiveUI;
namespace Moss.ViewModels;

public static class LocalTransfer
{

    #region Variables

    private static int _PageFromList;
    private static string _userToken;
    public static int _UserID;
    private static bool _CallUnderList;

    #endregion

    #region Properties
    public static int PageFromList
    {
        get => _PageFromList;
        set => _PageFromList = value;
    }
    public static bool CallUnderList
    {
        get => _CallUnderList;
        set => _CallUnderList = value;
    }
    public static int UserID
    {
        get => _UserID;
        set => _UserID = value;
    }
    public static string UserToken
    {
        get => _userToken;
        set => _userToken = value;
    }
    
    #endregion
    public static event Action PageFromListChangedEvent;
    public static void RaiseStaticEvent()
    {
        PageFromListChangedEvent?.Invoke();
    }
}