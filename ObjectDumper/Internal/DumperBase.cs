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
        private readonly Stack<int> listOfWrittenMembers;
        private readonly StringBuilder stringBuilder;
        private bool isNewLine;
        private bool AutoIndent;
        private int indentLevel;

        protected DumperBase(DumpOptions dumpOptions, bool autoIndent = true)
        {
            DumpOptions = dumpOptions;
            ObjectLevel = 0;
            indentLevel = 0;
            stringBuilder = new StringBuilder();
            listOfWrittenMembers = new Stack<int>();
            isNewLine = true;
            AutoIndent = autoIndent;
        }

        protected int ObjectLevel { get; private set; }
        protected bool InEnumerable { get => EnumerableLevel > 0; }
        protected int EnumerableLevel { get; private set; }
        protected int EnumerableIndex { get; private set; }
        protected DumpOptions DumpOptions { get; }
        protected bool IsMaxLevel { get => ObjectLevel > DumpOptions.MaxLevel; }
        protected int Indent
        {
            get => indentLevel;
            set => indentLevel = value < 0 ? 0 : value;
        }
        protected void DumpData(object value)
        {
            Dump(null, value);
        }

        private void Dump(MemberInfo property, object value)
        {
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
                default:
                    WriteObject(property, value);
                    break;
            }
        }

        private void WriteObject(MemberInfo property, object value)
        {
            if (IsMaxLevel)
            {
                WriteMaxLevel(value);
                return;
            }

            if (AlreadyWritten(value))
            {
                WriteCircularReference(value);
                return;
            }

            var type = value.GetType();
            var typeInfo = type.GetTypeInfo();
            ObjectLevel++;
            if (value is IEnumerable x)
            {
                DumpEnumerable(property, x);
            }
            else if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                DumpKeyValuePair(value, type);
            }
            else
            {
                DumpObject(value, type);
            }
            ObjectLevel--;
            listOfWrittenMembers.Pop();
        }

        /// <summary>
        /// Calls <see cref="Write(string, int)"/> using the current Level as indentLevel if the current
        /// position is at a the beginning of a new line or 0 otherwise. 
        /// </summary>
        /// <param name="value">string to be written</param>
        protected void Write(string value)
        {
            if (isNewLine)
            {
                Write(value, Indent);
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
            stringBuilder.Append(DumpOptions.IndentChar, indentLevel * DumpOptions.IndentSize);
            stringBuilder.Append(value);
            if (value.EndsWith(DumpOptions.LineBreakChar))
            {
                isNewLine = true;
            }
            else
            {
                isNewLine = false;
            }
        }

        /// <summary>
        /// Writes a line break to underlying <see cref="StringBuilder"/> using <see cref="DumpOptions.LineBreakChar"/>
        /// </summary>
        /// <remarks>
        /// By definition this sets isNewLine to true
        /// </remarks>
        protected void LineBreak(bool force = false)
        {
            if (!isNewLine)
            {
                stringBuilder.Append(DumpOptions.LineBreakChar);
                isNewLine = true;
            }
            else if (force && stringBuilder.Length > (2 * DumpOptions.LineBreakChar.Length))
            {
                if (!(DumpOptions.LineBreakChar + DumpOptions.LineBreakChar == stringBuilder.ToString(stringBuilder.Length - (DumpOptions.LineBreakChar.Length * 2), DumpOptions.LineBreakChar.Length * 2)))
                {
                    stringBuilder.Append(DumpOptions.LineBreakChar);
                }
            }
        }

        private bool AlreadyWritten(object value)
        {
            if (value == null)
            {
                return false;
            }

            var hash = value.GetHashCode();
            if (listOfWrittenMembers.Contains(hash))
            {
                return true;
            }
            else
            {
                listOfWrittenMembers.Push(hash);
                return false;
            }
        }

        private void DumpObject(object value, Type type)
        {
            WriteObjectStart(value, type);
            if (AutoIndent)
            {
                Indent++;
            }
            if (IsMaxLevel)
            {
                WriteMaxLevel(value);
            }
            else
            {
                var publicFields = value.GetType().GetRuntimeFields().Where(f => !f.IsPrivate);
                var properties = value.GetType().GetRuntimeProperties()
                   .Where(p => p.GetMethod != null && p.GetMethod.IsPublic && p.GetMethod.IsStatic == false);

                if (DumpOptions.ExcludeProperties != null && DumpOptions.ExcludeProperties.Any())
                {
                    properties = properties
                        .Where(p => !DumpOptions.ExcludeProperties.Contains(p.Name));

                }
                if (DumpOptions.SetPropertiesOnly)
                {
                    properties = properties
                        .Where(p => p.SetMethod != null && p.SetMethod.IsPublic && p.SetMethod.IsStatic == false);

                }
                if (DumpOptions.IgnoreDefaultValues)
                {
                    publicFields = publicFields
                        .Where(p =>
                        {
                            var currentValue = p.GetValue(value);
                            var defaultValue = p.FieldType.GetDefault();
                            var isDefaultValue = Equals(currentValue, defaultValue);
                            return !isDefaultValue;
                        });
                    properties = properties
                        .Where(p =>
                        {
                            var currentValue = p.GetValue(value);
                            var defaultValue = p.PropertyType.GetDefault();
                            var isDefaultValue = Equals(currentValue, defaultValue);
                            return !isDefaultValue;
                        });

                }
                if (DumpOptions.PropertyOrderBy != null)
                {
                    properties = properties.OrderBy(DumpOptions.PropertyOrderBy.Compile());
                }
                var propertyList = properties.ToList();
                var fieldList = publicFields.ToList();
                if (fieldList.Count > 0)
                {
                    WriteBeginFields();
                    for (int i = 0; i < fieldList.Count; i++)
                    {
                        var field = fieldList[i];
                        WriteFieldBegin(field);
                        Dump(field, field.TryGetValue(value));
                        WriteFieldEnd(field, i == (fieldList.Count - 1));
                    }
                    WriteEndFields();
                }
                if (propertyList.Count > 0)
                {
                    WriteBeginProperties();
                    for (int i = 0; i < propertyList.Count; i++)
                    {
                        var property = propertyList[i];
                        WritePropertyBegin(property);
                        Dump(property, property.TryGetValue(value));
                        WritePropertyEnd(property, i == (propertyList.Count - 1));
                    }
                    WriteEndProperties();
                }
            }
            if (AutoIndent)
            {
                Indent--;
            }

            WriteObjectEnd(value);
        }

        private void DumpKeyValuePair(object value, Type type)
        {
            var kvpKey = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Key));
            var kvpValue = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Value));

            WriteKeyValuePairBegin();
            if (IsMaxLevel)
            {
                WriteMaxLevel(value);
            }
            else
            {
                Dump(kvpKey, kvpKey.TryGetValue(value));
                WriteKeyValuePairSeperator();
                Dump(kvpValue, kvpValue.TryGetValue(value));
            }
            WriteKeyValuePairEnd();
        }

        private void DumpEnumerable(MemberInfo memberInfo, IEnumerable items)
        {
            EnumerableLevel++;
            EnumerableIndex = 0;
            WriteEnumerableBegin(memberInfo, items);
            if (AutoIndent)
            {
                Indent++;
            }
            if (IsMaxLevel)
            {
                WriteMaxLevel(items);
            }
            else
            {
                var e = items.GetEnumerator();
                if (e.MoveNext())
                {
                    int index = 0;
                    var item = e.Current;
                    while (e.MoveNext())
                    {
                        index++;
                        EnumerableIndex = index;
                        Dump(null, item);
                        item = e.Current;
                        WriteEnumerableSeperator(false);
                    }
                    index++;
                    EnumerableIndex = index;
                    Dump(null, item);
                    WriteEnumerableSeperator(true);
                }
            }
            if (AutoIndent)
            {
                Indent--;
            }

            WriteEnumerableEnd(memberInfo, items);
            EnumerableLevel--;
            EnumerableIndex = 0;
        }

        protected virtual void WriteObjectStart(object value, Type type) => Write("{");
        protected virtual void WriteObjectEnd(object value) => Write("}");
        protected virtual void WriteCircularReference(object value) => Write("null /*Recursive Object Found*/");
        protected virtual void WriteMaxLevel(object value) => Write("/*At Max Level*/");
        protected virtual void WriteEnumerableBegin(MemberInfo property, IEnumerable x) => Write("[");
        protected virtual void WriteEnumerableSeperator(bool lastItem) => Write(",");
        protected virtual void WriteEnumerableEnd(MemberInfo property, IEnumerable x) => Write("]");
        protected virtual void WriteKeyValuePairBegin() => Write("{");
        protected virtual void WriteKeyValuePairSeperator() => Write(",");
        protected virtual void WriteKeyValuePairEnd() => Write("}");
        protected virtual void WriteBeginProperties() => Write("{");
        protected virtual void WriteEndProperties() => Write("}");
        protected virtual void WritePropertyBegin(PropertyInfo property) => Write(property.Name);
        protected virtual void WritePropertyEnd(PropertyInfo property, bool lastProperty) => Write(",");
        protected virtual void WriteBeginFields() => Write("{");
        protected virtual void WriteEndFields() => Write("}");
        protected virtual void WriteFieldBegin(FieldInfo field) => Write(field.Name);
        protected virtual void WriteFieldEnd(FieldInfo field, bool lastField) => Write(",");
        protected virtual void WriteNull(MemberInfo property) => Write("null");
        protected virtual void WriteBool(MemberInfo property, bool value) => Write(value ? "true" : "false");
        protected virtual void WriteString(MemberInfo property, string value) => Write(value);
        protected virtual void WriteChar(MemberInfo property, char value) => Write(value.ToString());
        protected virtual void WriteDouble(MemberInfo property, double value) => Write(value.ToString());
        protected virtual void WriteDecimal(MemberInfo property, decimal value) => Write(value.ToString());
        protected virtual void WriteByte(MemberInfo property, byte value) => Write(value.ToString());
        protected virtual void WriteSbyte(MemberInfo property, sbyte value) => Write(value.ToString());
        protected virtual void WriteInt(MemberInfo property, int value) => Write(value.ToString());
        protected virtual void WriteFloat(MemberInfo property, float value) => Write(value.ToString());
        protected virtual void WriteUint(MemberInfo property, uint value) => Write(value.ToString());
        protected virtual void WriteLong(MemberInfo property, long value) => Write(value.ToString());
        protected virtual void WriteUlong(MemberInfo property, ulong value) => Write(value.ToString());
        protected virtual void WriteShort(MemberInfo property, short value) => Write(value.ToString());
        protected virtual void WriteUshort(MemberInfo property, ushort value) => Write(value.ToString());
        protected virtual void WriteDateTime(MemberInfo property, DateTime value) => Write(value.ToString());
        protected virtual void WriteEnum(MemberInfo property, Enum value) => Write(value.ToString());
        protected virtual void WriteGuid(MemberInfo property, Guid value) => Write(value.ToString());

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
