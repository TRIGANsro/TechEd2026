using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF2DBinding.Behaviors;

public class ZoomBehavior : Behavior<ScrollViewer>
{
    public static readonly DependencyProperty ZoomLevelProperty =
        DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(ZoomBehavior),
            new PropertyMetadata(1.0, OnZoomLevelChanged));

    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    private double _lastZoomLevel = 1.0;

    private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ZoomBehavior behavior)
        {
            behavior.ApplyZoom((double)e.OldValue, (double)e.NewValue);
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        AssociatedObject.Loaded -= OnLoaded;
        base.OnDetaching();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyZoom(_lastZoomLevel, ZoomLevel);
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;

            var oldZoom = ZoomLevel;
            var delta = e.Delta > 0 ? 0.1 : -0.1;
            var newZoom = Math.Clamp(ZoomLevel + delta, 0.1, 10.0);

            _lastZoomLevel = oldZoom;
            ZoomLevel = newZoom;
        }
    }

    private void ApplyZoom(double oldZoom, double newZoom)
    {
        if (AssociatedObject.Content is not FrameworkElement content)
            return;

        // Získat aktuální scroll pozice pøed zmìnou
        var oldHorizontalOffset = AssociatedObject.HorizontalOffset;
        var oldVerticalOffset = AssociatedObject.VerticalOffset;
        var viewportWidth = AssociatedObject.ViewportWidth;
        var viewportHeight = AssociatedObject.ViewportHeight;

        // Aplikovat scale transform
        var scaleTransform = new ScaleTransform(newZoom, newZoom);
        content.LayoutTransform = scaleTransform;

        // Poèkat na update layoutu
        content.UpdateLayout();

        // Vypoèítat novou scroll pozici - zoom na støed viditelné oblasti
        if (oldZoom > 0)
        {
            // Støed viewportu
            var centerX = viewportWidth / 2.0;
            var centerY = viewportHeight / 2.0;

            // Relativní pozice støedu v contentu pøed zoomem
            var relativeX = (oldHorizontalOffset + centerX) / oldZoom;
            var relativeY = (oldVerticalOffset + centerY) / oldZoom;

            // Nová scroll pozice tak, aby støed zùstal na stejném místì
            var newHorizontalOffset = (relativeX * newZoom) - centerX;
            var newVerticalOffset = (relativeY * newZoom) - centerY;

            // Nastavit novou scroll pozici
            AssociatedObject.ScrollToHorizontalOffset(newHorizontalOffset);
            AssociatedObject.ScrollToVerticalOffset(newVerticalOffset);
        }
    }
}