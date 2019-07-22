using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    internal class ObjectDumperCSharp : DumperBase
    {
        public ObjectDumperCSharp(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object value, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            var instance = new ObjectDumperCSharp(dumpOptions);
            instance.Write($"var {GetVariableName(value)} = ");
            instance.DumpData(value);
            instance.Write(";");
            return instance.ToString();
        }

        protected override void WriteObjectStart(object value)
        {
            Write($"new {GetClassName(value)}");
            LineBreak();
            Write("{");
            LineBreak();
        }

        protected override void WritePropertyBegin(PropertyInfo property)
        {
            Write($"{property.Name} = ");
        }

        protected override void WritePropertyEnd(PropertyInfo property, bool lastProperty)
        {
            if (!lastProperty)
            {
                Write(",");
            }
            LineBreak();
        }

        protected override void WriteString(PropertyInfo property, string value)
            => Write($"\"{value.Escape()}\"");

        protected override void WriteChar(PropertyInfo property, char value)
            => Write($"'{value.ToString().Escape()}'");

        protected override void WriteDouble(PropertyInfo property, double value)
            => Write($"{value}d");

        protected override void WriteDecimal(PropertyInfo property, decimal value)
            => Write($"{value}m");

        protected override void WriteFloat(PropertyInfo property, float value)
            => Write($"{value}f");

        protected override void WriteLong(PropertyInfo property, long value)
            => Write($"{value}L");

        protected override void WriteUlong(PropertyInfo property, ulong value)
            => Write($"{value}L");

        protected override void WriteEnum(PropertyInfo property, Enum value)
            => Write($"{value.GetType().FullName}.{value}");

        protected override void WriteGuid(PropertyInfo property, Guid value)
            => Write($"new Guid(\"{value:D}\")");

        protected override void WriteDateTime(PropertyInfo property, DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                Write($"DateTime.MinValue");
            }
            else if (value == DateTime.MaxValue)
            {
                Write($"DateTime.MaxValue");
            }
            else
            {
                Write($"DateTime.ParseExact(\"{value:O}\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)");
            }
        }

        protected override void WriteEnumerableBegin(PropertyInfo property, IEnumerable x)
        {
            Write($"new {GetClassName(x)}");
            LineBreak();
            Write("{");
            LineBreak();
        }

        protected override void WriteEnumerableSeperator(bool lastItem)
        {
            if (!lastItem)
            {
                Write(",");
            }
            LineBreak();
        }

        protected override void WriteEnumerableEnd(PropertyInfo property, IEnumerable x)
        {
            Write("}");
        }

        protected override void WriteKeyValuePairBegin() => Write("{ ");

        protected override void WriteKeyValuePairSeperator() => Write(", ");

        protected override void WriteKeyValuePairEnd() => Write(" }");

        protected override void WriteBeginProperties() { }
        protected override void WriteEndProperties() { }

        private static string GetClassName(object o)
        {
            var type = o.GetType();
            var className = type.GetFormattedName();
            return className;
        }

        private static string GetVariableName(object value)
        {
            if (value == null)
            {
                return "x";
            }

            var type = value.GetType();
            var variableName = type.Name;

            if (value is IEnumerable)
            {
                variableName = GetClassName(value)
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace(" ", "")
                    .Replace(",", "");
            }
            else if (type.GetTypeInfo().IsGenericType)
            {
                variableName = $"{type.Name.Substring(0, type.Name.IndexOf('`'))}";
            }

            return variableName.ToLowerFirst();
        }
    }
}
