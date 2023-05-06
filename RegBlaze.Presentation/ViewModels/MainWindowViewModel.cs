using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using RegBlaze.Core.Models;
using RegBlaze.Core.Services;

namespace RegBlaze.Presentation.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private int _completedTasks;
    private double _progrssBarValue;
    private ICollectionView _searchMatches;

    private int _totalTasks;

    public MainWindowViewModel(IServiceProvider serviceProvider,
        ITaskTracker searchTaskTracker)
    {
        _serviceProvider = serviceProvider;
        _searchMatches = new CollectionView(Enumerable.Empty<SearchMatch>());
        ExecuteSearchCommand = new AsyncRelayCommand<string>(ExecuteSearch);
        searchTaskTracker.TaskCompleted += OnTaskCompleted;
    }

    public double ProgressBarValue
    {
        get => _progrssBarValue;
        set
        {
            _progrssBarValue = value;
            OnPropertyChanged();
        }
    }

    public ICollectionView SearchMatches
    {
        get => _searchMatches;
        private set
        {
            _searchMatches = value;
            OnPropertyChanged();
        }
    }

    public AsyncRelayCommand<string> ExecuteSearchCommand { get; }
    public bool SearchInUsers { get; set; }
    public bool SearchInClassesRoot { get; set; }
    public bool SearchInCurrentConfig { get; set; }
    public bool SearchInCurrentUser { get; set; }
    public bool SearchInLocalMachine { get; set; }
    public bool SearchInPerformanceData { get; set; }

    private async void OnTaskCompleted(object? s, EventArgs a)
    {
        _completedTasks++;

        var newValue = (double)_completedTasks / _totalTasks * 100.0;

        for (var i = (int)ProgressBarValue; i < (int)newValue; i++)
        {
            ProgressBarValue = i;
            await Task.Delay(5);
        }

        ProgressBarValue = newValue;
    }

    private async Task OnSearchExecuted(int tasks)
    {
        _totalTasks = tasks;
        _completedTasks = 0;

        for (var i = ProgressBarValue; i > 0; i--)
        {
            ProgressBarValue--;
            await Task.Delay(5);
        }

        ProgressBarValue = 0;
    }

    private async Task ExecuteSearch(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return;
        var registryHives = GetRegistryHives();
        await OnSearchExecuted(registryHives.Count);
        if (registryHives.Count == 0) return;
        var searchService = _serviceProvider.GetRequiredService<IRegistrySearchService>();
        var result = await searchService.RunRegistrySearch(keyword, registryHives);
        SearchMatches = CollectionViewSource.GetDefaultView(result);
    }

    private List<RegistryHive> GetRegistryHives()
    {
        var list = new List<RegistryHive>();

        if (SearchInUsers) list.Add(RegistryHive.Users);
        if (SearchInClassesRoot) list.Add(RegistryHive.ClassesRoot);
        if (SearchInCurrentConfig) list.Add(RegistryHive.CurrentConfig);
        if (SearchInCurrentUser) list.Add(RegistryHive.CurrentUser);
        if (SearchInLocalMachine) list.Add(RegistryHive.LocalMachine);
        if (SearchInPerformanceData) list.Add(RegistryHive.PerformanceData);

        return list;
    }
}