using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class AreaInfo
    {
        public int area_no { get; set; }

        public string area_name { get; set; }

        public DateTime inputtime { get; set; }

        public string inputby { get; set; }

        public DateTime dLastUpdateTime { get; set; }
    }

    public class AreaInfoOutput
    {
        public int AreaNo { get; set; }

        public string AreaName { get; set; }

        public DateTime InputTime { get; set; }

        public string InputBy { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
