using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
public class DumpOptions
{
    public DumpOptions()
    {
        DumpStyle = DumpStyle.Console;
        IndentSize = 2;
        IndentChar = ' ';
        LineBreakChar = Environment.NewLine;
        SetPropertiesOnly = false;
        MaxLevel = int.MaxValue;
        ExcludeProperties = new HashSet<string>();
        PropertyOrderBy = null;
        IgnoreDefaultValues = false;
    }

    public DumpStyle DumpStyle { get; set; }

    public int IndentSize { get; set; }

    public char IndentChar { get; set; }

    public string LineBreakChar { get; set; }

    public bool SetPropertiesOnly { get; set; }

    public int MaxLevel { get; set; }

    public ICollection<string> ExcludeProperties { get; set; }

    public Expression<Func<PropertyInfo, object>> PropertyOrderBy { get; set; }

    public bool IgnoreDefaultValues { get; set; }
}
