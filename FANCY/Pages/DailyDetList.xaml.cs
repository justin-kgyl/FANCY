using FANCY.Models;
using FANCY.ProjectClass;
using KGYL.UTILITY;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DailyDetList : ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public DailyDetList(string invNo, decimal invProfit)
        {
            InitializeComponent();
            Title = "Order Detail List";
            Show_DailyDetList(invNo, invProfit);
        }



        public async void Show_DailyDetList(string invNo, decimal invProfit)
        {
            string userID = Preferences.Get("UserID", "XXXX");
            

            List<DailyDet> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            var client = new HttpClient();
            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);

            string url = $@"{endPoint}DailyDetList/{invNo}";
            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<DailyDet>>(response);


            DailyDetListView.ItemsSource = string.Empty;
            DailyDetListView.ItemsSource = records;

            int totQty = records.Sum(i => i.i_qty);
            lbl_totqty.Text = totQty.ToString();

            decimal totAmt = records.Sum(i => i.i_damt);
            lbl_totamt.Text = totAmt.ToString("C2");

            lbl_totprofit.Text = invProfit.ToString("C2");

            
            decimal nMARGIN = 0;
            if (invProfit == 0)
            {
                nMARGIN = 0;
            }
            else
            {
                nMARGIN = ((invProfit / totAmt) * 100);
            }
            lbl_totmargin.Text = nMARGIN.ToString("F2")+"%";


            //decimal amtProfit = records.Sum(i => i.i_profit);
            //decimal nMARGIN = 0;

            //if (amtSum != 0)
            //{
            //    nMARGIN = ((amtProfit / amtSum) * 100);
            //}

            //lbl_totamt.Text = $@"Total Amount : {amtSum.ToString("C2")}";
            //lbl_totprofit.Text = $@"Total Profit : {amtProfit.ToString("C2")}";
            //lbl_margin.Text = $@"Margin : {nMARGIN.ToString("F2")}%";
        }

        private void OnItemSelected(object sender, ItemTappedEventArgs e)
        {

        }
    }
}