using System;
using System.ComponentModel;
using System.Reflection;

namespace Puzzle.Masroofi.Core.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool IsEnumValid<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            return Enum.IsDefined(typeof(TEnum), enumValue);
        }

        public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            Type type = enumValue.GetType();
            FieldInfo fi = type.GetField(enumValue.ToString());
            var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attrs != null && attrs.Length > 0)
                return attrs[0].Description;
            else
                return enumValue.ToString();
        }
    }
}
