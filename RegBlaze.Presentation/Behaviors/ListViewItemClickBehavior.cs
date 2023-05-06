using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using RegBlaze.Core.Models;

namespace RegBlaze.Presentation.Behaviors;

public class ListViewItemClickBehavior : Behavior<ListView>
{
    protected override void OnAttached()
    {
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        base.OnAttached();
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (AssociatedObject.SelectedItem is not SearchMatch selectedItem) return;

        Clipboard.SetText(selectedItem.RegistryKey);
        /* bug => Needs to be fixed because of Windows 11 new ToastContentBuilder().AddText("1", hintMaxLines: 1).AddText("2").Show(); */
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
    }
}