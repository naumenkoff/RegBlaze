using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace RegBlaze.Presentation.Behaviors;

public class TextBlockClickBehavior : Behavior<TextBlock>
{
    protected override void OnAttached()
    {
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        base.OnAttached();
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        Clipboard.SetText(AssociatedObject.Text);
        /* bug => Needs to be fixed because of Windows 11 new ToastContentBuilder().AddText("1", hintMaxLines: 1).AddText("2").Show(); */
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
    }
}