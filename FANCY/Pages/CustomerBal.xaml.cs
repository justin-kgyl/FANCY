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
    public partial class CustomerBal : ContentPage
    {

        KGYLDatabase LocalDb = new KGYLDatabase();
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public CustomerBal()
        {
            InitializeComponent();
            Title = "A/R Unpaid Report";
            Show_Cust();
        }

        public async void Show_Cust()
        {

            string userID = Preferences.Get("UserID", "XXXX");
            string salesCat = Preferences.Get("SalesCat", "XXXX");

            List<araging> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            var client = new HttpClient();

            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);

            string url = $@"{endPoint}ARAging/{userID}?par2={salesCat}";
            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<araging>>(response);

            decimal totBalQty = records.Sum(x => x.C_BALANCE);
            lbl_totbal.Text = string.Format("{0:C1}", totBalQty);

            CustBalListView.ItemsSource = string.Empty;
            CustBalListView.ItemsSource = records;
        }
    }
}