using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Mi.Fish.Common
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值定义的 DisplayName
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string DisplayName(this Enum value)
        {
            var field = GetEnumField(value);

            var displayName = field.DisplayName();
            if (!string.IsNullOrEmpty(displayName))
            {
                return displayName;
            }

            var attribute = field.GetCustomAttribute<DisplayNameAttribute>();

            return attribute?.DisplayName ?? value.ToString();
        }

        /// <summary>
        /// 获取枚举值定义的 Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            var field = GetEnumField(value);

            var description = field.DisplayDescription();

            return description ?? "";
        }

        private static MemberInfo GetEnumField(Enum value)
        {
            var enumType = value.GetType();
            var name = value.ToString();
            return  enumType.GetField(name);
        }
    }
}
