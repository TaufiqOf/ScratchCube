using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                return;

            field = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<PropertyItem> Properties { get; } = new();

    public void SetInspect(T? inspect)
    {
        Inspect = inspect;

        if (inspect == null)
        {
            if (Properties.Count > 0)
                Properties.Clear();

            return;
        }

        var values = new List<PropertyValue>();

        var visited = new HashSet<object>(
            ReferenceComparer.Instance);

        AppendValue(
            nameof(T),
            inspect,
            0,
            visited,
            values);

        UpdateProperties(values);
    }

    private void UpdateProperties(List<PropertyValue> values)
    {
        /*
         * If the structure hasn't changed, replace the PropertyItem
         * at the same index. This causes ObservableCollection to notify
         * the DataGrid without requiring PropertyItem to implement
         * INotifyPropertyChanged.
         */

        var sameStructure =
            Properties.Count == values.Count &&
            Properties
                .Select((x, i) => x.Path == values[i].Path)
                .All(x => x);

        if (sameStructure)
        {
            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];

                if (Properties[i].Value != value.Value)
                {
                    Properties[i] = new PropertyItem(
                        value.Path,
                        value.Value,
                        value.Depth);
                }
            }

            return;
        }

        /*
         * Structure changed.
         *
         * Instead of rebuilding the ObservableCollection object,
         * update the existing collection.
         */

        Properties.Clear();

        foreach (var value in values)
        {
            Properties.Add(
                new PropertyItem(
                    value.Path,
                    value.Value,
                    value.Depth));
        }
    }

    private void AppendValue(
        string path,
        object? value,
        int depth,
        HashSet<object> visited,
        List<PropertyValue> result)
    {
        if (value == null)
        {
            result.Add(
                new PropertyValue(
                    path,
                    "null",
                    depth));

            return;
        }

        var type = value.GetType();

        if (IsSimple(type))
        {
            result.Add(
                new PropertyValue(
                    path,
                    FormatSimple(value),
                    depth));

            return;
        }

        if (value is IDictionary dictionary)
        {
            result.Add(
                new PropertyValue(
                    path,
                    $"dict({dictionary.Count})",
                    depth));

            foreach (DictionaryEntry entry in dictionary)
            {
                var keyText = entry.Key == null
                    ? "null"
                    : FormatSimple(entry.Key);

                AppendValue(
                    $"{path}[{keyText}]",
                    entry.Value,
                    depth + 1,
                    visited,
                    result);
            }

            return;
        }

        if (value is IEnumerable enumerable and not string)
        {
            var items = enumerable
                .Cast<object?>()
                .ToList();

            result.Add(
                new PropertyValue(
                    path,
                    $"list({items.Count})",
                    depth));

            for (var i = 0; i < items.Count; i++)
            {
                AppendValue(
                    $"{path}[{i}]",
                    items[i],
                    depth + 1,
                    visited,
                    result);
            }

            return;
        }

        if (!type.IsValueType && !visited.Add(value))
        {
            result.Add(
                new PropertyValue(
                    path,
                    "<circular-reference>",
                    depth));

            return;
        }

        result.Add(
            new PropertyValue(
                path,
                $"object({type.Name})",
                depth));

        var properties = type
            .GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance)
            .Where(p =>
                p.CanRead &&
                p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.Name);

        foreach (var property in properties)
        {
            object? propertyValue;

            try
            {
                propertyValue =
                    property.GetValue(value);
            }
            catch (Exception ex)
            {
                propertyValue =
                    $"<error:{ex.GetType().Name}>";
            }

            AppendValue(
                property.Name,
                propertyValue,
                depth + 1,
                visited,
                result);
        }

        if (!type.IsValueType)
            visited.Remove(value);
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
            DateTime time =>
                time.ToString("O"),

            DateTimeOffset time =>
                time.ToString("O"),

            bool boolean =>
                boolean ? "true" : "false",

            _ =>
                value.ToString() ?? string.Empty
        };
    }

    private sealed class ReferenceComparer
        : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Instance = new();

        bool IEqualityComparer<object>.Equals(
            object? x,
            object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    private sealed record PropertyValue(
        string Path,
        string Value,
        int Depth);
}

public sealed class PropertyItem
{
    public PropertyItem(
        string path,
        string value,
        int depth)
    {
        Path = path;
        Value = value;
        Depth = depth;

        DisplayPath =
            $"{new string(' ', depth * 2)}{path}";
    }

    public string Path { get; }

    public string DisplayPath { get; }

    public string Value { get; }

    public int Depth { get; }
}