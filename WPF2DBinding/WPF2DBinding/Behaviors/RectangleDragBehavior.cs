using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF2DBinding.Models;

namespace WPF2DBinding.Behaviors;

public class RectangleDragBehavior : Behavior<Canvas>
{
    public static readonly DependencyProperty ZajimavostiProperty =
        DependencyProperty.Register(nameof(Zajimavosti), typeof(System.Collections.ObjectModel.ObservableCollection<Zajimavost>),
            typeof(RectangleDragBehavior), new PropertyMetadata(null, OnZajimavostiChanged));

    public static readonly DependencyProperty SelectedZajimavostProperty =
        DependencyProperty.Register(nameof(SelectedZajimavost), typeof(Zajimavost),
            typeof(RectangleDragBehavior), new PropertyMetadata(null, OnSelectedZajimavostChanged));

    public static readonly DependencyProperty ZoomLevelProperty =
        DependencyProperty.Register(nameof(ZoomLevel), typeof(double),
            typeof(RectangleDragBehavior), new PropertyMetadata(1.0, OnZoomLevelChanged));

    public System.Collections.ObjectModel.ObservableCollection<Zajimavost>? Zajimavosti
    {
        get => (System.Collections.ObjectModel.ObservableCollection<Zajimavost>)GetValue(ZajimavostiProperty);
        set => SetValue(ZajimavostiProperty, value);
    }

    public Zajimavost? SelectedZajimavost
    {
        get => (Zajimavost)GetValue(SelectedZajimavostProperty);
        set => SetValue(SelectedZajimavostProperty, value);
    }

    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    private Point? _dragStartPoint;
    private Rectangle? _currentRectangle;
    private Point _rectangleStartPoint;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        
        if (Zajimavosti != null)
        {
            Zajimavosti.CollectionChanged -= OnZajimavostiCollectionChanged;
        }
        
        base.OnDetaching();
    }

    private static void OnZajimavostiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RectangleDragBehavior behavior)
        {
            if (e.OldValue is System.Collections.ObjectModel.ObservableCollection<Zajimavost> oldCollection)
            {
                oldCollection.CollectionChanged -= behavior.OnZajimavostiCollectionChanged;
            }
            
            if (e.NewValue is System.Collections.ObjectModel.ObservableCollection<Zajimavost> newCollection)
            {
                newCollection.CollectionChanged += behavior.OnZajimavostiCollectionChanged;
            }
            
            behavior.RedrawRectangles();
        }
    }

    private static void OnSelectedZajimavostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RectangleDragBehavior behavior)
        {
            behavior.UpdateAllRectangleColors();
        }
    }

    private void OnZajimavostiCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RedrawRectangles();
    }

    private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RectangleDragBehavior behavior)
        {
            behavior.UpdateStrokeThickness();
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Kreslení pouze pokud je nìco vybráno
        if (SelectedZajimavost == null)
            return;

        if (AssociatedObject.IsMouseDirectlyOver)
        {
            _dragStartPoint = e.GetPosition(AssociatedObject);
            _rectangleStartPoint = _dragStartPoint.Value;

            _currentRectangle = new Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = GetAdjustedStrokeThickness(),
                Fill = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255))
            };

            AssociatedObject.Children.Add(_currentRectangle);
            AssociatedObject.CaptureMouse();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStartPoint.HasValue && _currentRectangle != null)
        {
            var currentPoint = e.GetPosition(AssociatedObject);

            var x = Math.Min(_rectangleStartPoint.X, currentPoint.X);
            var y = Math.Min(_rectangleStartPoint.Y, currentPoint.Y);
            var width = Math.Abs(currentPoint.X - _rectangleStartPoint.X);
            var height = Math.Abs(currentPoint.Y - _rectangleStartPoint.Y);

            Canvas.SetLeft(_currentRectangle, x);
            Canvas.SetTop(_currentRectangle, y);
            _currentRectangle.Width = width;
            _currentRectangle.Height = height;
        }
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragStartPoint.HasValue && _currentRectangle != null)
        {
            var x = Canvas.GetLeft(_currentRectangle);
            var y = Canvas.GetTop(_currentRectangle);
            var width = _currentRectangle.Width;
            var height = _currentRectangle.Height;

            AssociatedObject.Children.Remove(_currentRectangle);

            // Aktualizace vybraného obdélníku
            if (width > 5 && height > 5 && SelectedZajimavost != null)
            {
                SelectedZajimavost.Oblast = new Rect(x, y, width, height);
                RedrawRectangles();
            }

            _dragStartPoint = null;
            _currentRectangle = null;
            AssociatedObject.ReleaseMouseCapture();
        }
    }

    private void RedrawRectangles()
    {
        AssociatedObject.Children.Clear();

        if (Zajimavosti == null) return;

        foreach (var zajimavost in Zajimavosti)
        {
            // Pøeskoèit obdélníky s nulovou nebo velmi malou plochou
            if (zajimavost.Oblast.Width < 1 || zajimavost.Oblast.Height < 1)
                continue;

            var rect = new Rectangle
            {
                Width = zajimavost.Oblast.Width,
                Height = zajimavost.Oblast.Height,
                StrokeThickness = GetAdjustedStrokeThickness(),
                Tag = zajimavost
            };

            UpdateRectangleAppearance(rect, zajimavost);

            Canvas.SetLeft(rect, zajimavost.Oblast.X);
            Canvas.SetTop(rect, zajimavost.Oblast.Y);

            rect.MouseLeftButtonDown += (s, e) =>
            {
                SelectedZajimavost = zajimavost;
                e.Handled = true;
            };

            zajimavost.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Zajimavost.Oblast))
                {
                    RedrawRectangles();
                }
                else if (e.PropertyName == nameof(Zajimavost.IsSelected))
                {
                    UpdateRectangleAppearance(rect, zajimavost);
                }
            };

            AssociatedObject.Children.Add(rect);
        }
    }

    private void UpdateAllRectangleColors()
    {
        if (Zajimavosti == null) return;

        foreach (var child in AssociatedObject.Children)
        {
            if (child is Rectangle rect && rect.Tag is Zajimavost zajimavost)
            {
                UpdateRectangleAppearance(rect, zajimavost);
            }
        }
    }

    private void UpdateRectangleAppearance(Rectangle rect, Zajimavost zajimavost)
    {
        var isSelected = zajimavost == SelectedZajimavost;
        
        if (isSelected)
        {
            rect.Stroke = Brushes.Blue;
            rect.Fill = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255));
        }
        else
        {
            rect.Stroke = Brushes.Red;
            rect.Fill = new SolidColorBrush(Color.FromArgb(30, 255, 0, 0));
        }
    }

    private void UpdateStrokeThickness()
    {
        var thickness = GetAdjustedStrokeThickness();
        
        foreach (var child in AssociatedObject.Children)
        {
            if (child is Rectangle rect)
            {
                rect.StrokeThickness = thickness;
            }
        }
    }

    private double GetAdjustedStrokeThickness()
    {
        return Math.Clamp(2.0 / ZoomLevel, 0.5, 5.0);
    }
}