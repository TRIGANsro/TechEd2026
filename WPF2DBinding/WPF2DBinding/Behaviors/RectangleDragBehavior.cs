using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF2DBinding.Models;

namespace WPF2DBinding.Behaviors;

/// <summary>
/// Behavior pro kreslení nového obdélníku drag & drop operací.
/// Pouze zachytává myš a aktualizuje Oblast vybrané Zajímavost.
/// </summary>
public class RectangleDragBehavior : Behavior<Canvas>
{
    public static readonly DependencyProperty SelectedZajimavostProperty =
        DependencyProperty.Register(nameof(SelectedZajimavost), typeof(Zajimavost),
            typeof(RectangleDragBehavior), new PropertyMetadata(null));

    public Zajimavost? SelectedZajimavost
    {
        get => (Zajimavost)GetValue(SelectedZajimavostProperty);
        set => SetValue(SelectedZajimavostProperty, value);
    }

    private Point? _dragStartPoint;
    private DragRectangleAdorner? _adorner;
    private AdornerLayer? _adornerLayer;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        AssociatedObject.Loaded -= OnLoaded;
        
        RemoveAdorner();
        base.OnDetaching();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Kreslení pouze pokud je nìco vybráno
        if (SelectedZajimavost == null || _adornerLayer == null)
            return;

        if (AssociatedObject.IsMouseDirectlyOver)
        {
            _dragStartPoint = e.GetPosition(AssociatedObject);

            // Vytvoøit adorner pro preview obdélník
            _adorner = new DragRectangleAdorner(AssociatedObject, _dragStartPoint.Value);
            _adornerLayer.Add(_adorner);
            
            AssociatedObject.CaptureMouse();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStartPoint.HasValue && _adorner != null)
        {
            var currentPoint = e.GetPosition(AssociatedObject);
            _adorner.UpdateRectangle(_dragStartPoint.Value, currentPoint);
        }
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragStartPoint.HasValue && _adorner != null)
        {
            var rect = _adorner.GetRectangle();
            
            RemoveAdorner();

            // Aktualizace vybrané zajímavosti - WPF binding se postará o zbytek
            if (rect.Width > 5 && rect.Height > 5 && SelectedZajimavost != null)
            {
                SelectedZajimavost.Oblast = rect;
            }

            _dragStartPoint = null;
            AssociatedObject.ReleaseMouseCapture();
        }
    }

    private void RemoveAdorner()
    {
        if (_adorner != null && _adornerLayer != null)
        {
            _adornerLayer.Remove(_adorner);
            _adorner = null;
        }
    }
}

/// <summary>
/// Adorner pro zobrazení preview obdélníku bìhem drag operace.
/// </summary>
internal class DragRectangleAdorner : Adorner
{
    private Rect _rectangle;
    private readonly Pen _pen;
    private readonly Brush _fill;

    public DragRectangleAdorner(UIElement adornedElement, Point startPoint) : base(adornedElement)
    {
        _rectangle = new Rect(startPoint, startPoint);
        
        _pen = new Pen(Brushes.Blue, 2)
        {
            DashStyle = new DashStyle(new double[] { 4.0, 2.0 }, 0)
        };
        _pen.Freeze();
        
        _fill = Brushes.Transparent;
        
        IsHitTestVisible = false; // Adorner nepøekáží mouse eventùm
    }

    public void UpdateRectangle(Point startPoint, Point currentPoint)
    {
        var x = Math.Min(startPoint.X, currentPoint.X);
        var y = Math.Min(startPoint.Y, currentPoint.Y);
        var width = Math.Abs(currentPoint.X - startPoint.X);
        var height = Math.Abs(currentPoint.Y - startPoint.Y);

        _rectangle = new Rect(x, y, width, height);
        InvalidateVisual(); // Pøekreslit adorner
    }

    public Rect GetRectangle() => _rectangle;

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        drawingContext.DrawRectangle(_fill, _pen, _rectangle);
    }
}