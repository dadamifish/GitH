using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Mi.Fish.Common
{
    public static class DataTableExtension
    {
        public static string GetValue(this DataTable dt, string filter)
        {
            var row = dt.Select(filter);
            if (row.Length > 0)
            {
                return row[0]["kindvalue"].ToString();
            }
            return "";
        }
    }
}
