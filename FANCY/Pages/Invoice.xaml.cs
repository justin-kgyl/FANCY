using FANCY.Models;
using FANCY.ProjectClass;
using KGYL.UTILITY;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Invoice : ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();
        string _CustID = "";
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public Invoice(string par1)
        {
            InitializeComponent();
            _CustID = par1.Trim();
            Title = "Invoice List";
            Show_Inv();
        }

        public async void Show_Inv()
        {
  
            List<invlist> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            string url = "";
            var client = new HttpClient();

            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);

            url = $@"{endPoint}INVLIST/{_CustID}";

            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<invlist>>(response);


            InvListView.ItemsSource = string.Empty;
            InvListView.ItemsSource = records;

            
        }

        private async void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as invlist;

            await Navigation.PushAsync(new InvoiceDetail(myDetail.i_id));

        }

    }
}