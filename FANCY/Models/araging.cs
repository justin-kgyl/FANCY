using System;
using System.Collections.Generic;
using System.Text;

namespace FANCY.Models
{
    public class araging
    {
        public string C_ID { get; set; }
        public string C_NAME { get; set; }
        public string C_ADDRESS { get; set; }
        public string C_CITY { get; set; }
        public string C_STATE { get; set; }
        public string C_ZIP { get; set; }
        public decimal C_BALANCE { get; set; }

        public string FullAddress
        {
            get
            {
                string cReturn = C_ADDRESS.Trim() + " " + C_CITY.Trim() + ", " + C_STATE.Trim() + " " + C_ZIP.Trim();
                return cReturn;
            }
        }
    }
}
