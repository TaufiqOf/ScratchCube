using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchDocker.ViewModels;

namespace ScratchDocker.Models.Docker;

public partial class DockerVolume : ViewModelBase
{
    public Action<DockerVolume>? OnDelete { get; set; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _driver = string.Empty;

    [ObservableProperty]
    private string _mountpoint = string.Empty;

    [ObservableProperty]
    private string _scope = string.Empty;

    [ObservableProperty]
    private string _createdAt = string.Empty;

    [ObservableProperty]
    private long _refCount;

    [ObservableProperty]
    private long _size;

    public string DisplaySize
    {
        get
        {
            if (Size <= 0)
            {
                return "-";
            }

            var size = (double)Size;
            string[] units = ["B", "KB", "MB", "GB", "TB"];
            var unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size.ToString("0.##", CultureInfo.InvariantCulture)} {units[unitIndex]}";
        }
    }

    public void Update(DockerVolume dockerVolume)
    {
        Name = dockerVolume.Name;
        Driver = dockerVolume.Driver;
        Mountpoint = dockerVolume.Mountpoint;
        Scope = dockerVolume.Scope;
        CreatedAt = dockerVolume.CreatedAt;
        RefCount = dockerVolume.RefCount;
        Size = dockerVolume.Size;
    }
    [RelayCommand]
    public void Delete()
    {
        OnDelete?.Invoke(this);
    }
}

