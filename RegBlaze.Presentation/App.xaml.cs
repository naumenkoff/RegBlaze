using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RegBlaze.Domain;
using RegBlaze.Infrastructe;
using RegBlaze.Presentation.Models;
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

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(provider => new MainWindow
        {
            DataContext = provider.GetRequiredService<MainWindowViewModel>()
        });
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<ITaskTracker, TaskTracker>();
        services.AddSingleton<ScanningOptions>();

        services.AddSingleton<Func<string, IRegistrySearchService>>(sp => keyword =>
        {
            var registryKeyProcessorFactory = sp.GetRequiredService<Func<string, IRegistryKeyProcessor>>();
            var registryKeyProcessor = registryKeyProcessorFactory(keyword);
            return new RegistrySearchService(registryKeyProcessor, sp.GetRequiredService<ITaskTracker>());
        });

        services.AddSingleton<Func<string, IRegistryKeyProcessor>>(sp => keyword =>
        {
            var keywordCheckerFactory = sp.GetRequiredService<Func<string, IKeywordChecker>>();
            var keywordChecker = keywordCheckerFactory(keyword);
            return new RegistryKeyProcessor(keywordChecker);
        });
        services.AddSingleton<Func<string, IKeywordChecker>>(_ => keyword => new KeywordChecker(keyword));

        return services.BuildServiceProvider();
    }
}