using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProjectPilotLite.Avalonia.Services;
using ProjectPilotLite.Avalonia.ViewModels;

namespace ProjectPilotLite.Avalonia;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5286/") };
            var api = new ApiClient(httpClient);
            var viewModel = new DashboardViewModel(api);

            desktop.MainWindow = new MainWindow { DataContext = viewModel };
            _ = viewModel.LoadAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
