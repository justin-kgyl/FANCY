using FANCY.Models;
using KGYL.UTILITY;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FANCY.ProjectClass
{
    class KGYLDatabase
    {
        public SQLiteAsyncConnection db = null;

        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public KGYLDatabase()
        {
            string databaseName = "KGYL.db3";
            string databasePath = "";

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    SQLitePCL.Batteries_V2.Init();
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName);
                    break;
                case Device.Android:
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
                    break;
                default:
                    throw new NotImplementedException("Platform not supported");
            }

            db = new SQLiteAsyncConnection(databasePath);
        }

		
		public async Task<int> CreateAndUpdateTable<T>(string func, string userID = "", string par2 = "") where T : new()
        {
            int ret_val = -1;
            List<T> records = null;

            try
            {
                var client = new HttpClient();
                string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
                client.DefaultRequestHeaders.Add("KGYLKey", key);

                string url = $@"{endPoint}{func}/{userID}?par2={par2}";
                //string url = endPoint + func + "/";
                var response = await client.GetStringAsync(url);
                records = JsonConvert.DeserializeObject<List<T>>(response);
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }

            string modelName = typeof(T).Name;

            if (await TableExists(modelName))
            {
                await db.DropTableAsync<T>();
            }

            await db.CreateTableAsync<T>();

            List<T> inserts = new List<T>();

            foreach (var record in records)
            {
                inserts.Add(record);
            }

            await db.InsertAllAsync(inserts);

            ret_val = inserts.Count;

            return ret_val;
        }
		
        
        public async Task<bool> TableExists(String tableName)
        {
            bool ret_val = false;

            SQLite.TableMapping map = new TableMapping(typeof(SqlDbType));
            object[] ps = new object[0];

            var tableCount = await db.QueryAsync(map, "SELECT * FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "'", ps);
            if (tableCount.Count == 1)
            {
                ret_val = true;
            }

            return ret_val;
        }

        public async void Zap_Table(string tableName)
        {
            string cSQL = "delete from " + tableName.Trim();
            await db.ExecuteAsync(cSQL);
        }

        public async Task<CUST> GetCUST(string id)
        {
            string cSQL = $@"SELECT * 
                             FROM CUST 
                             WHERE C_ID ='{id}'";
            var custs = await db.QueryAsync<CUST>(cSQL);
            CUST cust = custs.FirstOrDefault();

            return cust;
        }

        public async Task<bool> IsCartEmpty()
        {
            var rs = await db.Table<CART>().OrderBy(c => c.i_cust).ToListAsync();
            int rCount = rs.Count();

            if (rCount == 0)
            {
                return true;
            }
            else return false;
        }

        public async Task<bool> IsRemarkEmpty()
        {
            var rs = await db.Table<REMARK>().OrderBy(r => r.r_cust).ToListAsync();
            int rCount = rs.Count();          

            if (rCount == 0)
            {
				return true;
            }
            else return false;
        }
		
		public async Task<decimal> GetTotalAmt(string custID)
        {
            var cart = await db.Table<CART>().Where(c => c.i_cust == custID).ToListAsync();
            decimal total = cart.Sum(i => i.i_damt);
            return total;

        }

        public async Task<decimal> GetTotalQty(string custID)
        {
            var cart = await db.Table<CART>().Where(c => c.i_cust == custID).ToListAsync();
            decimal total = cart.Sum(i => i.i_qty);
            return total;

        }

        public async Task<string> SubmitOrder(string custID, string userID)
        {
            string ret_value = "NO";

            string cSQL = $@" SELECT i_item, i_qty, i_desc1, i_unit, i_size, i_pri, i_damt, i_cost, i_profit, i_cust, i_user, i_comm 
                              FROM CART 
                              WHERE i_cust = '{custID}' 
                              ORDER BY i_item";
            var cart = db.QueryAsync<CART>(cSQL).Result.ToList();

            string rSQL = $@" SELECT r_cust, r_rem1, r_rem2, r_rem3, r_rem4 
                              FROM REMARK 
                              WHERE r_cust ='{custID}'";
            var rm = db.QueryAsync<REMARK>(rSQL).Result.FirstOrDefault();

            string rem1 = rm.r_rem1 ?? "";
            string rem2 = rm.r_rem2 ?? "";
            string rem3 = rm.r_rem3 ?? "";
            string rem4 = rm.r_rem4 ?? "";

            string uri = $@"{endPoint}UpdateOrder/{HttpUtility.UrlEncode(custID)}?userID={HttpUtility.UrlEncode(userID)}&rem1={HttpUtility.UrlEncode(rem1)}&rem2={HttpUtility.UrlEncode(rem2)}&rem3={HttpUtility.UrlEncode(rem3)}&rem4={HttpUtility.UrlEncode(rem4)}";
            //uri = HttpUtility.UrlEncode(uri);

            using (var client = new HttpClient())
            {                
                try
                {
                    string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
                    client.DefaultRequestHeaders.Add("KGYLKey", key);
                    HttpResponseMessage res = await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json"));

                    if (res.IsSuccessStatusCode)
                    {
                        string ret_val1 = await res.Content.ReadAsStringAsync();

                        string[] arrays = ret_val1.Split(':');
                        string invNo = arrays[0];
                        int insQty = int.Parse(arrays[1]);

                        if (insQty == decimal.ToInt32(await GetTotalQty(custID)))
                        {
                            string sql = $@"DELETE FROM CART WHERE i_cust = '{custID}'";
                            await db.ExecuteAsync(sql);

                            sql = $@"DELETE FROM REMARK WHERE r_cust = '{custID}'";
                            await db.ExecuteAsync(sql);

                            ret_value = $@"Submitted And Invoice - [{invNo}] created Successfully !";
                        }
                        else
                        {
                            ret_value = $@"Ouch! Only {insQty} Items Successfully Submitted. Please Check Invoice [{invNo}] @";
                        }
                    }

                }
                catch (Exception ex)
                {
                    //ret_value = ex.Message;
                    ret_value = "Error";                    
                }

                return ret_value;
            }

        }
    }
}
