using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RegBlaze.Core.Models;
using RegBlaze.Core.Services;
using RegBlaze.Presentation.ViewModels;
using RegBlaze.Presentation.Views;

namespace RegBlaze.Presentation;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        _serviceProvider = ConfigureServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var main = _serviceProvider.GetRequiredService<MainWindow>();
        main.Show();
        base.OnStartup(e);
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(provider => new MainWindow
            { DataContext = provider.GetRequiredService<MainWindowViewModel>() });
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<IRegistryKeyProcessor, RegistryKeyProcessor>();
        services.AddSingleton<IRegistrySearchService, RegistrySearchService>();
        services.AddSingleton<IKeywordChecker, KeywordChecker>();
        services.AddSingleton<ITaskTracker, TaskTracker>();

        return services.BuildServiceProvider();
    }
}