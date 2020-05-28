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
    public partial class DailyList : ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public DailyList()
        {
            InitializeComponent();
            Title = "Daily Order List";
            Show_DailyList();
        }



        public async void Show_DailyList()
        {
            string userID = Preferences.Get("UserID", "XXXX");
            string salesCat = Preferences.Get("SalesCat", "XXXX");

            List<inv> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            var client = new HttpClient();
            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);

            string url = $@"{endPoint}DailyList/{userID}?salesCat={salesCat}";
            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<inv>>(response);


            DailyListView.ItemsSource = string.Empty;
            DailyListView.ItemsSource = records;

            decimal amtSum = records.Sum(i => i.i_invamt);
            decimal amtProfit = records.Sum(i => i.i_profit);
            decimal nMARGIN = 0;

            if (amtSum != 0)
            {
                nMARGIN = ((amtProfit / amtSum) * 100);
            }

            lbl_totamt.Text = $@"Total Amount : {amtSum.ToString("C2")}";
            lbl_totprofit.Text = $@"Total Profit : {amtProfit.ToString("C2")}";
            lbl_margin.Text = $@"Margin : {nMARGIN.ToString("F2")}%";
        }

        private async void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as inv;
            
            Page pAGE = new DailyDetList(myDetail.i_invno, myDetail.i_profit);

            await Navigation.PushAsync(pAGE);
        }
    }
}