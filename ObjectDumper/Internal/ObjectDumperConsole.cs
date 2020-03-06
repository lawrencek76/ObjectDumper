using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    internal class ObjectDumperConsole : DumperBase
    {
        public ObjectDumperConsole(DumpOptions dumpOptions) : base(dumpOptions, false)
        {
        }

        public static string Dump(object element, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            var instance = new ObjectDumperConsole(dumpOptions);
            instance.DumpData(element);
            return instance.ToString();
        }

        protected override void WriteEnumerableBegin(MemberInfo property, IEnumerable x)
        {
            if (ObjectLevel > 1)
            {
                Write("...");
                Indent++;
                LineBreak();
            }
        }
        protected override void WriteEnumerableSeperator(bool lastItem)
        {
            if (!lastItem)
            {
                LineBreak();
            }
        }
        protected override void WriteEnumerableEnd(MemberInfo property, IEnumerable x)
        {
            if (ObjectLevel > 1)
            {
                Indent--;
            }
            LineBreak();
        }
        protected override void WriteObjectStart(object value, Type type)
        {
            if (EnumerableIndex>1)
            {
                LineBreak(true);
            }
            if (!InEnumerable && ObjectLevel > 1)
            {
                Write("{ }");
                LineBreak();
                Indent++;
            }
            Write($"{{{type.GetFormattedName(useFullName: true)}}}");
            LineBreak();
            Indent++;
        }
        protected override void WriteObjectEnd(object value)
        {
            Indent--;
            if (!InEnumerable && ObjectLevel > 1)
            {
                Indent--;
            }
        }
        protected override void WriteCircularReference(object value)
        {
            Write("<-- bidirectional reference found");
        }
        protected override void WriteMaxLevel(object value) { }
        protected override void WriteKeyValuePairBegin() { Write("["); }
        protected override void WriteKeyValuePairEnd() { Write("]"); }
        protected override void WriteKeyValuePairSeperator() { Write(", "); }
        protected override void WritePropertyBegin(PropertyInfo property)
        {
            Write($"{property.Name}: ");
        }
        protected override void WritePropertyEnd(PropertyInfo property, bool lastProperty)
        {
            LineBreak();
        }
        protected override void WriteBeginProperties() { }
        protected override void WriteEndProperties() { }
        protected override void WriteFieldBegin(FieldInfo field)
        {
            Write($"{field.Name}: ");
        }
        protected override void WriteFieldEnd(FieldInfo field, bool lastProperty)
        {
            LineBreak();
        }
        protected override void WriteBeginFields() { }
        protected override void WriteEndFields() { }

        protected override void WriteString(MemberInfo property, string value)
        {
            Write($"\"{value}\"");
        }
        protected override void WriteChar(MemberInfo property, char value)
        {
            Write($"{value.ToString()}");
        }
        protected override void WriteBool(MemberInfo property, bool value)
        {
            if (value)
            {
                Write("True");
            }
            else
            {
                Write("False");
            }
        }
    }
}
