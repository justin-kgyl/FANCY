using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FANCY.Models
{
    class CART
    {
        [PrimaryKey, AutoIncrement]
        public int i_num { get; set; }
        public string i_item { get; set; }
        public string i_desc1 { get; set; }
        public decimal i_qty { get; set; }
        public string i_unit { get; set; }
        public string i_size { get; set; }
        public int i_onhand { get; set; }
        public decimal i_pri { get; set; }
        public decimal i_cost { get; set; }
        public decimal i_damt { get; set; }
        public decimal i_profit { get; set; }
        public decimal i_comm { get; set; }
        public string i_cust { get; set; }
        public string i_user { get; set; }
        public string i_cartdisplay1 { get; set; }
        public string i_cartdisplay2 { get; set; }
        public DateTime? i_oderdate { get; set; }     
    }
}
