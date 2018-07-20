using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class OrderPrint
    {
       public List<GoodsPrint> GoodsList { get; set; }
        public string flow_no { get; set; }
        public string deskno { get; set; }
        public string Other1 { get; set; }
    }

    public class GoodsPrint
    {
        public string item_name { get; set; }
        public decimal sale_qnty { get; set; }
    }
}
