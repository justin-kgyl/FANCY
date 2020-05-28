
using KGYL.GENERAL;
using System;
using System.Security.Cryptography;
using System.Text;

namespace KGYL.UTILITY
{
    public static class KGYLUtility
    {
        public static string GetMd5Hash(this string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string GetSha256Hash(this string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sBuilder.Append(bytes[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public static string KGYLPasswordGenerator(this DateTime date)         
        {
            // date =  04/16/2020

            string ret_val = "";

            string strDate = date.ToString("MM/dd/yyyy");                   // 04/16/2020
            string strMonth = strDate.Substring(0, 2);                      // 04    
            string strDay = strDate.Substring(3, 2);                        // 16

            string strM1 = strMonth.Substring(1, 1);                        // 4    
            string strD1 = strDay.Substring(1, 1);                          // 6    

            int n1 = Convert.ToInt32(strM1);
            int n2 = Convert.ToInt32(strD1);
            int n3 = n1 + n2;                                               

            string strN3 = n3.ToString();                                   // 10

            int n3_1 = 0;
            int n3_2 = 0;

            if (strN3.Length > 1)
            {
                string strN3_1 = strN3.Substring(0, 1);
                string strN3_2 = strN3.Substring(1, 1);

                n3_1 = Convert.ToInt32(strN3_1);
                n3_2 = Convert.ToInt32(strN3_2);
            }
            else
            {
                string strN3_1 = strN3.Substring(0, 1);
                n3_1 = Convert.ToInt32(strN3_1) + n2;
            }

            int n4 = n3_1 + n3_2;                                           // 1

            ret_val = strMonth + strDay + n3.ToString() + n4.ToString();    // 0416101

            return ret_val;
        }

        public static bool KGYLPassword(this string password)
        {
            bool IsKGYL = false;
            
            DateTime dt = DateTime.Now;

            string strKey = dt.KGYLPasswordGenerator();

            if (strKey == password)
            {
                IsKGYL = true;
            }

            return IsKGYL;
        }

    }
   
}