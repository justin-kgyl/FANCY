using System;
using System.Collections.Generic;
using System.Text;

namespace FANCY.Models
{
    public class CUST
    {
        public string C_ID { get; set; }
        public string C_NAME { get; set; }
        public string C_ADDRESS { get; set; }
        public string C_CITY { get; set; }
        public string C_STATE { get; set; }
        public string C_ZIP { get; set; }
        public string C_TEL { get; set; }
        public string C_SALESREP { get; set; }
        public string C_TYPE { get; set; }
        public string C_REGION { get; set; }
        public string C_EMAIL { get; set; }
        public string C_TERM { get; set; }
        public string C_ATTN { get; set; }
        public decimal C_BALANCE { get; set; }
        public string C_ACTIVE { get; set; }

        public string FullCustName
        {
            get
            {
                string cReturn = C_NAME.Trim() + " [" + C_ID.Trim() + "] " + "    Balance: " + C_BALANCE.ToString("C");
                return cReturn;
            }
        }
		
		public string FullAddress
        {
            get
            {
                string cReturn = C_ADDRESS.Trim() + " " + C_CITY.Trim() + ", " + C_STATE.Trim() + " " + C_ZIP.Trim(); 
                return cReturn;
            }
        }
    }

    public class CUST_NAME
    {
        public string C_NAME { get; set; }
    }


}
