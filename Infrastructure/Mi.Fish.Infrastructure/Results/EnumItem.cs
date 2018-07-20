

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Mi.Fish.Common;

namespace Mi.Fish.Infrastructure.Results
{
    /// <summary>
    /// 枚举值和展示名称
    /// </summary>
    public class EnumItem
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public EnumItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        /// 文本
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }

    public static class EnumItemFactory
    {
        /// <summary>
        /// Gets the enum items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">如果传入类型不是枚举类型产生异常</exception>
        public static List<EnumItem> GetEnumItems<T>()
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{type.FullName} is not a Enum type.");
            }

            var values = type.GetEnumValues().Cast<T>().ToList();

            //order 
            var dic = new Dictionary<T, int>();
            
            foreach (var value in values)
            {
                var field = type.GetField(value.ToString());
                var order = field.DisplayOrder();
                if (order.HasValue)
                {
                    dic.Add(value, order.Value);
                }
            }

            if (dic.Any())
            {
                var orderedValues = dic.OrderBy(o => o.Value).Select(o => o.Key).ToList();
                var orignals = values.Except(orderedValues).ToList();
                orderedValues.AddRange(orignals);

                values = orderedValues;
            }

            return GetEnumItems(values.ToArray());
        }

        /// <summary>
        /// Gets the enum items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enums">The enums.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">如果传入类型不是枚举类型产生异常</exception>
        public static List<EnumItem> GetEnumItems<T>(params T[] enums)
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{type.FullName} is not a Enum type.");
            }

            return enums.Distinct().Select(o => new EnumItem((o as Enum).DisplayName(), o.ToString())).ToList();
        }
    }
}
