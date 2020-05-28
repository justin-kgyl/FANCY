using FANCY.Models;
using FANCY.ProjectClass;
using KGYL.UTILITY;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InvoiceDetail : ContentPage
    {
        string _InvNo = "";
        ProjectClass.KGYLDatabase LocalDb = new KGYLDatabase();
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public InvoiceDetail(string par1)
        {
            InitializeComponent();
            _InvNo = par1.Trim();
            Title = "Invoice Detail List ("+ _InvNo+")" ;
            Show_inv();
            Show_Invdet();

        }

        public void Show_inv()
        {
  
            //lbl_invno.Text = "Invoice No : " + _InvNo;
            // lbl_cust.Text = "";

            //sql = "";
            //sql += "SELECT C_NAME ";
            //sql += "FROM INV ";
            //sql += "LEFT OUTER JOIN CUST ON CUST.C_ID = INV.I_BILL ";
            //sql += "WHERE I_INVNO  = '"+ _InvNo + "'";

            //var list = LocalDb.db.QueryAsync<CUST_NAME>(sql).Result.ToList();

        }

        public async void Show_Invdet()
        {


            List<invdetlist> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            string url = "";
            var client = new HttpClient();
            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);

            url = $@"{endPoint}INVDETLIST/{_InvNo}";

            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<invdetlist>>(response);

            decimal totQty = records.Sum(x => x.i_qty);
            lbl_totQty.Text = totQty.ToString().Trim();

            decimal totDamt = records.Sum(x => x.i_damt);
            lbl_totDamt.Text = string.Format("{0:C}", totDamt);

            decimal profit = records[0].i_profit;
            lbl_profit.Text = string.Format("{0:C}", profit);

            decimal nMARGIN ;

            if (profit == 0)
            {
                nMARGIN = 0.00m;
            }
            else
            {
                nMARGIN = Math.Round(((profit / totDamt) * 100),2);
            }

            lbl_margin.Text = nMARGIN.ToString().Trim() + "%";


            InvdetListView.ItemsSource = string.Empty;
            InvdetListView.ItemsSource = records;

        }
    }
}