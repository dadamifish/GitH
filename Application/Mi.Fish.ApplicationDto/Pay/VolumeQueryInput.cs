using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 礼券
    /// </summary>
    public class VolumeQueryInput
    {
        /// <summary>
        /// 礼券号
        /// </summary>
        [Required]
        public string VolumeNo { get; set; }


    }
}
