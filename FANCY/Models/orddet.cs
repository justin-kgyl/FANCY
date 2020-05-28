using System;
using System.Collections.Generic;
using System.Text;

namespace FANCY.Models
{
    class orddet
    {
        public string od_id { get; set; }
        public string od_desc1 { get; set; }
        public string od_desc2 { get; set; }
        public string od_class { get; set; }
        public string od_size { get; set; }
        public string od_unit { get; set; }
        public decimal od_pri { get; set; }
        public decimal od_minpri { get; set; }
        public int od_onhand { get; set; }
        public int od_qty { get; set; }
        public decimal? od_cost { get; set; }
        public decimal od_damt { get; set; }
        public decimal od_profit { get; set; }
        public decimal od_comm { get; set; }
        public string od_cust { get; set; }
        public string od_user { get; set; }
        public string od_color { get; set; }
    }
}
