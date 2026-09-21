using System;
using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScratchCube.ViewModels;

namespace ScratchCube.Models.Docker;

public partial class DockerImage : ViewModelBase
{
    public Action<DockerImage>? OnDelete { get; set; }

    [ObservableProperty] private string _id = string.Empty;

    [ObservableProperty] private IList<string> _repoTags = new List<string>();

    [ObservableProperty] private DateTime _created;

    [ObservableProperty] private long _size;

    [ObservableProperty] private long _containers;

    public string DisplayName => RepoTags.Count > 0 ? RepoTags[0] : "<none>:<none>";

    public string ShortId =>
        Id.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase)
            ? Id.Substring(7, Math.Min(12, Id.Length - 7))
            : Id[..Math.Min(12, Id.Length)];

    public DateTime CreatedAt => Created;
    
    public override string ToString()
    {
        return DisplayName;
    }

    public string DisplaySize
    {
        get
        {
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

    public void Update(DockerImage dockerImage)
    {
        Id = dockerImage.Id;
        RepoTags = dockerImage.RepoTags;
        Created = dockerImage.Created;
        Size = dockerImage.Size;
        Containers = dockerImage.Containers;
    }

    [RelayCommand]
    public void Delete()
    {
        OnDelete?.Invoke(this);
    }

}

