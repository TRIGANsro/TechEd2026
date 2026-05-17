using System.Windows;
using WpfAzureConfig.ViewModels;

namespace WpfAzureConfig;

/// <summary>
/// Main window - uses dependency injection to receive ViewModel.
/// Demonstrates MVVM pattern with proper separation of concerns.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Constructor with dependency injection of ViewModel
    /// </summary>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        
        // Set DataContext to injected ViewModel
        DataContext = viewModel;
    }
}