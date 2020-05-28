using System;
using System.Collections.Generic;
using System.Text;

namespace FANCY.Models
{
    public class DailyDet
    {
        public string i_id { get; set; }
        public string i_bill { get; set; }
        public string i_item { get; set; }
        public string i_desc1 { get; set; }
        public DateTime i_invdate { get; set; }
        public int i_qty { get; set; }
        public decimal i_pri { get; set; }
        public decimal i_damt { get; set; }
    }
}
