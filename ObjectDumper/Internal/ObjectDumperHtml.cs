using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ObjectDumping.Internal
{
    /// <summary>
    ///     
    /// </summary>
    internal class ObjectDumperHtml : DumperBase
    {
        public ObjectDumperHtml(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object value, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            var instance = new ObjectDumperHtml(dumpOptions);
            var type = GetTypeName(value);
            instance.Write($"<div class={CssClass("dump", "type-" + type)}>");

            instance.Write($"<div class={CssClass("type-info")}>");
            instance.Write($"<span class={CssClass("label")}>Dumped Type:</span> ");
            instance.Write($"<span class={CssClass("type")}>{System.Net.WebUtility.HtmlEncode(type)}</span>");
            instance.Write("</div>");
            instance.DumpData(value);
            instance.Write("</div>");
            return instance.ToString();
        }

        protected override void WriteObjectStart(object value, Type type)
        {
            Write($"<div class={"type-" + GetTypeName(value)}>");
        }

        protected override void WriteObjectEnd(object value)
        {
            Write("</div>");
        }

        protected override void WritePropertyBegin(PropertyInfo property)
        {
            Write($"<div class={CssClass("type-" + property?.PropertyType?.GetFormattedName() ?? "null", property.Name)}><span class={CssClass("label")}>{property.Name}</span> ");
        }

        protected override void WritePropertyEnd(PropertyInfo property, bool lastProperty)
        {
            Write($"</div>");
        }

        protected override void WriteBool(MemberInfo property, bool value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteByte(MemberInfo property, byte value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteChar(MemberInfo property, char value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteDecimal(MemberInfo property, decimal value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteDouble(MemberInfo property, double value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }
        protected override void WriteEnum(MemberInfo property, Enum value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteFloat(MemberInfo property, float value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteGuid(MemberInfo property, Guid value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteInt(MemberInfo property, int value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteLong(MemberInfo property, long value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteNull(MemberInfo property)
        {
            Write($"<span class={CssClass("value", "null")}>null</span>");
        }

        protected override void WriteSbyte(MemberInfo property, sbyte value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteShort(MemberInfo property, short value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteString(MemberInfo property, string value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteUint(MemberInfo property, uint value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteUlong(MemberInfo property, ulong value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }

        protected override void WriteUshort(MemberInfo property, ushort value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString()}</span>");
        }
        protected override void WriteDateTime(MemberInfo property, DateTime value)
        {
            Write($"<span class={CssClass("value")}>{value.ToString("o")}</span>");
        }

        private static Regex cssRegex = new Regex("[^A-z0-9_-]");
        private static string CssClass(params string[] classes)
        {
            StringBuilder result = new StringBuilder();
            result.Append("\"");
            for (int i = 0; i < classes.Length; i++)
            {
                result.Append("obj-");
                result.Append(cssRegex.Replace(classes[i].Replace('<', '-'), string.Empty));
                if (i < classes.Length - 1)
                {
                    result.Append(" ");
                }
            }
            result.Append("\"");

            return result.ToString();
        }

        private static string GetTypeName(object o)
        {
            var type = o.GetType();
            var name = type.GetFormattedName();
            return name;
        }
    }
}
