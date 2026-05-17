using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace WPF2DBinding.Models;

public partial class Zajimavost : ObservableObject
{
    [ObservableProperty]
    private Guid id = Guid.NewGuid();

    [ObservableProperty]
    private string nadpis = string.Empty;

    [ObservableProperty]
    private string popis = string.Empty;

    [ObservableProperty]
    private Rect oblast;

    [ObservableProperty]
    private bool isSelected;
}