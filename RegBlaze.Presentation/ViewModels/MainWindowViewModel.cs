using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RegBlaze.Domain;
using RegBlaze.Presentation.Models;

namespace RegBlaze.Presentation.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly Func<string, IRegistrySearchService> _registrySearcherServiceFactory;

    private int _completedTasks;
    private double _progrssBarValue;
    private ICollectionView _searchMatches;
    private int _totalTasks;

    public MainWindowViewModel(ScanningOptions scanningOptions, Func<string, IRegistrySearchService> registrySearcherServiceFactory,
        ITaskTracker searchTaskTracker)
    {
        Options = scanningOptions;
        _registrySearcherServiceFactory = registrySearcherServiceFactory;
        _searchMatches = new CollectionView(Enumerable.Empty<SearchMatch>());
        ExecuteSearchCommand = new AsyncRelayCommand<string>(ExecuteSearch);
        searchTaskTracker.TaskCompleted += OnTaskCompleted;
    }

    public ScanningOptions Options { get; }

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

    private async void OnTaskCompleted(object? s, EventArgs a)
    {
        _completedTasks++;

        var newValue = (double) _completedTasks / _totalTasks * 100.0;

        for (var i = (int) ProgressBarValue; i < (int) newValue; i++)
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

        var hives = Options.GetRegistryHives();
        if (hives.Count == 0) return;

        await OnSearchExecuted(hives.Count);
        var searchService = _registrySearcherServiceFactory(keyword);
        var result = await searchService.ExecuteSearch(hives);

        SearchMatches = CollectionViewSource.GetDefaultView(result);
    }
}