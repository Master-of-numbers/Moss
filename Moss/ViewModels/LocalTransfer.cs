using System;
using ReactiveUI;
namespace Moss.ViewModels;

public static class LocalTransfer
{

    public static event Action PageFromListChangedEvent;
    public static event Action AuthPageChangedEventAct;
    public static void AuthPageChangedEvent()
    {
        AuthPageChangedEventAct?.Invoke();
    }
    public static void RaiseStaticEvent()
    {
        PageFromListChangedEvent?.Invoke();
    }

    private static int _RedCirclePos;
    private static string _userToken;
    public static int RedCirclePos
    {
        get => _RedCirclePos;
        set => _RedCirclePos = value;
    }
    private static int _WhiteCirclePos;
    public static int WhiteCirclePos
    {
        get => _WhiteCirclePos;
        set => _WhiteCirclePos = value;
    }
    private static int _PageFromList;
    public static int PageFromList
    {
        get => _PageFromList;
        set => _PageFromList = value;
    }

    private static bool _CallUnderList;
    public static bool CallUnderList
    {
        get => _CallUnderList;
        set => _CallUnderList = value;
    }
    public static int _UserID;
    public static int UserID
    {
        get => _UserID;
        set => _UserID = value;
    }

    public static string _UserName;

    public static string UserName
    {
        get => _UserName;
        set => _UserName = value;
    }
    public static string UserToken
    {
        get => _userToken;
        set => _userToken = value;
    }
}