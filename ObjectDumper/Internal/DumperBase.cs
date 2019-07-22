using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectDumping.Internal
{
    public abstract class DumperBase
    {
        private readonly List<int> hashListOfFoundElements;
        private readonly StringBuilder stringBuilder;
        private bool isNewLine;

        protected DumperBase(DumpOptions dumpOptions)
        {
            this.DumpOptions = dumpOptions;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
            this.isNewLine = true;
        }

        public int Level { get; set; }

        public bool IsMaxLevel()
        {
            return this.Level > this.DumpOptions.MaxLevel;
        }

        protected DumpOptions DumpOptions { get; }

        /// <summary>
        /// Calls <see cref="Write(string, int)"/> using the current Level as indentLevel if the current
        /// position is at a the beginning of a new line or 0 otherwise. 
        /// </summary>
        /// <param name="value">string to be written</param>
        protected void Write(string value)
        {
            if (this.isNewLine)
            {
                Write(value, Level);
            }
            else
            {
                Write(value, 0);
            }
        }

        /// <summary>
        /// Writes value to underlying <see cref="StringBuilder"/> using <paramref name="indentLevel"/> and <see cref="DumpOptions.IndentChar"/> and <see cref="DumpOptions.IndentSize"/>
        /// to determin the indention chars prepended to <paramref name="value"/>
        /// </summary>
        /// <remarks>
        /// This function needs to keep up with if the last value written included the LineBreakChar at the end
        /// </remarks>
        /// <param name="value">string to be written</param>
        /// <param name="indentLevel">number of indentions to prepend default 0</param>
        protected void Write(string value, int indentLevel = 0)
        {
            this.stringBuilder.Append(this.DumpOptions.IndentChar, indentLevel * this.DumpOptions.IndentSize);
            this.stringBuilder.Append(value);
            if (value.EndsWith(this.DumpOptions.LineBreakChar))
            {
                this.isNewLine = true;
            }
            else
            {
                this.isNewLine = false;
            }
        }

        /// <summary>
        /// Writes a line break to underlying <see cref="StringBuilder"/> using <see cref="DumpOptions.LineBreakChar"/>
        /// </summary>
        /// <remarks>
        /// By definition this sets isNewLine to true
        /// </remarks>
        protected void LineBreak()
        {
            this.stringBuilder.Append(this.DumpOptions.LineBreakChar);
            isNewLine = true;
        }

        protected void AddAlreadyTouched(object element)
        {
            this.hashListOfFoundElements.Add(element.GetHashCode());
        }

        protected bool AlreadyTouched(object value)
        {
            if (value == null)
            {
                return false;
            }

            var hash = value.GetHashCode();
            for (var i = 0; i < this.hashListOfFoundElements.Count; i++)
            {
                if (this.hashListOfFoundElements[i] == hash)
                {
                    return true;
                }
            }

            return false;
        }

        protected void DumpData(object value)
        {
            DumpElement(null, value);
        }

        private void DumpObject(object value)
        {
            var type = value.GetType();
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                DumpKeyValuePair(value, type);
                return;
            }

            var properties = value.GetType().GetRuntimeProperties()
               .Where(p => p.GetMethod != null && p.GetMethod.IsPublic && p.GetMethod.IsStatic == false);

            if (this.DumpOptions.ExcludeProperties != null && this.DumpOptions.ExcludeProperties.Any())
            {
                properties = properties
                    .Where(p => !this.DumpOptions.ExcludeProperties.Contains(p.Name));

            }
            if (this.DumpOptions.SetPropertiesOnly)
            {
                properties = properties
                    .Where(p => p.SetMethod != null && p.SetMethod.IsPublic && p.SetMethod.IsStatic == false);

            }
            if (this.DumpOptions.IgnoreDefaultValues)
            {
                properties = properties
                    .Where(p =>
                    {
                        var currentValue = p.GetValue(value);
                        var defaultValue = p.PropertyType.GetDefault();
                        var isDefaultValue = Equals(currentValue, defaultValue);
                        return !isDefaultValue;
                    });

            }
            if (this.DumpOptions.PropertyOrderBy != null)
            {
                properties = properties.OrderBy(this.DumpOptions.PropertyOrderBy.Compile());
            }
            WriteObjectStart(value);
            this.Level++;
            WriteBeginProperties();
            var propertyList = properties.ToList();
            for (int i = 0; i < propertyList.Count; i++)
            {
                var property = propertyList[i];
                WritePropertyBegin(property);
                DumpElement(property, property.TryGetValue(value));
                WritePropertyEnd(property, i == (propertyList.Count - 1));
            }
            WriteEndProperties();
            this.Level--;
            WriteObjectEnd(value);
        }

        private void DumpKeyValuePair(object value, Type type)
        {
            var kvpKey = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Key));
            var kvpValue = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Value));

            WriteKeyValuePairBegin();
            DumpElement(kvpKey, kvpKey.TryGetValue(value));
            WriteKeyValuePairSeperator();
            DumpElement(kvpValue, kvpValue.TryGetValue(value));
            WriteKeyValuePairEnd();
        }

        private void DumpElement(PropertyInfo property, object value)
        {
            if (this.IsMaxLevel())
            {
                return;
            }

            switch (value)
            {
                case null:
                    WriteNull(property);
                    break;
                case bool x:
                    WriteBool(property, x);
                    break;
                case string x:
                    WriteString(property, x);
                    break;
                case char x:
                    WriteChar(property, x);
                    break;
                case double x:
                    WriteDouble(property, x);
                    break;
                case decimal x:
                    WriteDecimal(property, x);
                    break;
                case byte x:
                    WriteByte(property, x);
                    break;
                case sbyte x:
                    WriteSbyte(property, x);
                    break;
                case float x:
                    WriteFloat(property, x);
                    break;
                case int x:
                    WriteInt(property, x);
                    break;
                case uint x:
                    WriteUint(property, x);
                    break;
                case long x:
                    WriteLong(property, x);
                    break;
                case ulong x:
                    WriteUlong(property, x);
                    break;
                case short x:
                    WriteShort(property, x);
                    break;
                case ushort x:
                    WriteUshort(property, x);
                    break;
                case DateTime x:
                    WriteDateTime(property, x);
                    break;
                case Enum x:
                    WriteEnum(property, x);
                    break;
                case Guid x:
                    WriteGuid(property, x);
                    break;
                case IEnumerable x:
                    WriteEnumerableBegin(property, x);
                    DumpEnumerable(x);
                    WriteEnumerableEnd(property, x);
                    break;
                default:
                    DumpObject(value);
                    break;
            }
        }

        private void DumpEnumerable(IEnumerable items)
        {
            this.Level++;
            if (this.IsMaxLevel())
            {
                this.Level--;
                return;
            }
            
            var e = items.GetEnumerator();
            if (e.MoveNext())
            {
                var item = e.Current;
                while (e.MoveNext())
                {
                    DumpElement(null, item);
                    item = e.Current;
                    WriteEnumerableSeperator(false);
                }
                DumpElement(null, item);
                WriteEnumerableSeperator(true);
            }
            
            this.Level--;
        }

        protected virtual void WriteObjectStart(object value) => Write("{");
        protected virtual void WriteObjectEnd(object value) => Write("}");
        protected virtual void WriteEnumerableBegin(PropertyInfo property, IEnumerable x) => Write("[");
        protected virtual void WriteEnumerableSeperator(bool lastItem) => Write(",");
        protected virtual void WriteEnumerableEnd(PropertyInfo property, IEnumerable x) => Write("]");
        protected virtual void WriteKeyValuePairBegin() => Write("{");
        protected virtual void WriteKeyValuePairSeperator() => Write(",");
        protected virtual void WriteKeyValuePairEnd() => Write("}");
        protected virtual void WriteBeginProperties() => Write("{");
        protected virtual void WriteEndProperties() => Write("}");
        protected virtual void WritePropertyBegin(PropertyInfo property) => Write("{");
        protected virtual void WritePropertyEnd(PropertyInfo property, bool lastProperty) => Write("}");
        protected virtual void WriteNull(PropertyInfo property) => Write("null");
        protected virtual void WriteBool(PropertyInfo property, bool value) => Write(value ? "true" : "false");
        protected virtual void WriteString(PropertyInfo property, string value) => Write(value);
        protected virtual void WriteChar(PropertyInfo property, char value) => Write(value.ToString());
        protected virtual void WriteDouble(PropertyInfo property, double value) => Write(value.ToString());
        protected virtual void WriteDecimal(PropertyInfo property, decimal value) => Write(value.ToString());
        protected virtual void WriteByte(PropertyInfo property, byte value) => Write(value.ToString());
        protected virtual void WriteSbyte(PropertyInfo property, sbyte value) => Write(value.ToString());
        protected virtual void WriteInt(PropertyInfo property, int value) => Write(value.ToString());
        protected virtual void WriteFloat(PropertyInfo property, float value) => Write(value.ToString());
        protected virtual void WriteUint(PropertyInfo property, uint value) => Write(value.ToString());
        protected virtual void WriteLong(PropertyInfo property, long value) => Write(value.ToString());
        protected virtual void WriteUlong(PropertyInfo property, ulong value) => Write(value.ToString());
        protected virtual void WriteShort(PropertyInfo property, short value) => Write(value.ToString());
        protected virtual void WriteUshort(PropertyInfo property, ushort value) => Write(value.ToString());
        protected virtual void WriteDateTime(PropertyInfo property, DateTime value) => Write(value.ToString());
        protected virtual void WriteEnum(PropertyInfo property, Enum value) => Write(value.ToString());
        protected virtual void WriteGuid(PropertyInfo property, Guid value) => Write(value.ToString());

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }
    }
}
