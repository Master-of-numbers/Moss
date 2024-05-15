using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Moss.ViewModels;

namespace Moss.Views;

public partial class UserPageView : UserControl
{
    public UserPageView()
    {
        InitializeComponent();
    }

    private void TemplatedControl_OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        LocalTransfer.CallUnderList = false;
        throw new System.NotImplementedException();
    }
}