using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mi.Fish.Common;

namespace Mi.Fish.Infrastructure.Results
{
    /// <summary>
    /// 枚举类型返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.List{Mi.Fish.Infrastructure.Results.EnumItem}" />
    public class EnumResult<T> : List<EnumItem>
    {
        public EnumResult() : base(EnumItemFactory.GetEnumItems<T>())
        {

        }

        public EnumResult(params T[] enums) : base(EnumItemFactory.GetEnumItems(enums))
        {

        }
    }
}
