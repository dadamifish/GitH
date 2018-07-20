using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 礼券状态更新输入参数
    /// </summary>
    public  class VolumeUpdateInput
    {
        /// <summary>
        /// 礼券号集合
        /// </summary>
        [Required]
        public string[] VolumeNos { get; set; }

        /// <summary>
        /// 礼券状态更新 0 更新为已使用 2 撤销更新（变为未使用）
        /// </summary>

        public int Status { get; set; }

    }
}
