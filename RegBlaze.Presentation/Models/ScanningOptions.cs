using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;

namespace RegBlaze.Presentation.Models;

public class ScanningOptions : ObservableObject
{
    private readonly List<RegistryHive> _hives = new List<RegistryHive>();

    private bool _classesRoot;
    private bool _currentConfig;
    private bool _currentUser;
    private bool _localMachine;
    private bool _performanceData;
    private bool _users;

    public bool ClassesRoot
    {
        get => _classesRoot;
        set
        {
            _classesRoot = ChangeHives(RegistryHive.ClassesRoot, value);
            OnPropertyChanged();
        }
    }

    public bool CurrentConfig
    {
        get => _currentConfig;
        set
        {
            _currentConfig = ChangeHives(RegistryHive.CurrentConfig, value);
            OnPropertyChanged();
        }
    }

    public bool CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = ChangeHives(RegistryHive.CurrentUser, value);
            OnPropertyChanged();
        }
    }

    public bool LocalMachine
    {
        get => _localMachine;
        set
        {
            _localMachine = ChangeHives(RegistryHive.LocalMachine, value);
            OnPropertyChanged();
        }
    }

    public bool PerformanceData
    {
        get => _performanceData;
        set
        {
            _performanceData = ChangeHives(RegistryHive.PerformanceData, value);
            OnPropertyChanged();
        }
    }

    public bool Users
    {
        get => _users;
        set
        {
            _users = ChangeHives(RegistryHive.Users, value);
            OnPropertyChanged();
        }
    }

    public IReadOnlyCollection<RegistryHive> GetRegistryHives()
    {
        return _hives;
    }

    private bool ChangeHives(RegistryHive registryHive, bool value)
    {
        if (value)
            _hives.Add(registryHive);
        else
            _hives.Remove(registryHive);

        return value;
    }
}