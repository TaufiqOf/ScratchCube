using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ScratchDocker.Models.Docker;

namespace ScratchDocker.ViewModels.Controls;

public class ModelViewerControlViewModel<T> : ViewModelBase
{
    public ModelViewerControlViewModel(T? inspect)
    {
        SetInspect(inspect);
    }

    public T? Inspect
    {
        get;
        private set
        {
            if (Equals(value, field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PropertyItem> Properties { get; } = new();

    public void SetInspect(T? inspect)
    {
        Inspect = inspect;
        Properties.Clear();

        if (inspect == null)
        {
            return;
        }

        var visited = new HashSet<object>(ReferenceComparer.Instance);
        AppendValue(nameof(T), inspect, 0, visited);
    }

    private void AppendValue(string path, object? value, int depth, HashSet<object> visited)
    {
        if (value == null)
        {
            Properties.Add(new PropertyItem(path, "null", depth));
            return;
        }

        var type = value.GetType();

        if (IsSimple(type))
        {
            Properties.Add(new PropertyItem(path, FormatSimple(value), depth));
            return;
        }

        if (value is IDictionary dictionary)
        {
            Properties.Add(new PropertyItem(path, $"dict({dictionary.Count})", depth));

            foreach (DictionaryEntry entry in dictionary)
            {
                var keyText = entry.Key == null ? "null" : FormatSimple(entry.Key);
                AppendValue($"{path}[{keyText}]", entry.Value, depth + 1, visited);
            }

            return;
        }

        if (value is IEnumerable enumerable and not string)
        {
            var items = enumerable.Cast<object?>().ToList();
            Properties.Add(new PropertyItem(path, $"list({items.Count})", depth));

            for (var i = 0; i < items.Count; i++)
            {
                AppendValue($"{path}[{i}]", items[i], depth + 1, visited);
            }

            return;
        }

        if (!type.IsValueType && !visited.Add(value))
        {
            Properties.Add(new PropertyItem(path, "<circular-reference>", depth));
            return;
        }

        Properties.Add(new PropertyItem(path, $"object({type.Name})", depth));

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.Name);

        foreach (var property in properties)
        {
            object? propertyValue;

            try
            {
                propertyValue = property.GetValue(value);
            }
            catch (Exception ex)
            {
                propertyValue = $"<error:{ex.GetType().Name}>";
            }

            AppendValue($"{property.Name}", propertyValue, depth + 1, visited);
        }

        if (!type.IsValueType)
        {
            visited.Remove(value);
        }
    }

    private static bool IsSimple(Type type)
    {
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(TimeSpan)
               || type == typeof(Guid);
    }

    private static string FormatSimple(object value)
    {
        return value switch
        {
            DateTime time => time.ToString("O"),
            DateTimeOffset time => time.ToString("O"),
            bool boolean => boolean ? "true" : "false",
            _ => value.ToString() ?? string.Empty
        };
    }

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Instance = new();

        bool IEqualityComparer<object>.Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}

public sealed class PropertyItem
{
    public PropertyItem(string path, string value, int depth)
    {
        Path = path;
        Value = value;
        Depth = depth;
        DisplayPath = $"{new string(' ', depth * 2)}{path}";
    }

    public string Path { get; }
    public string DisplayPath { get; }
    public string Value { get; }
    public int Depth { get; }
}



