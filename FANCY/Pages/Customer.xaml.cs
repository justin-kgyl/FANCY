using FANCY.Models;
using FANCY.ProjectClass;
using KGYL.UTILITY;
using Newtonsoft.Json;
using Plugin.Messaging;
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
    public partial class Customer : ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";

        public Customer()
        {
            InitializeComponent();
            Title = "Customer List";
            Show_Custlist("All");
        }

        private void ShowCustomer_Clicked(object sender, System.EventArgs e)
        {
            Show_Custlist("");
        }

        public async void Show_Custlist(string par1)
        {

            string userID   = Preferences.Get("UserID", "XXXX");
            string salesCat = Preferences.Get("SalesCat", "XXXX");
            string search = txtCustName.Text.ToUpper().Trim();
 
            List<CUST> records = null;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Iternet Connection", "ok");
                return;
            }

            string url = "";
            var client = new HttpClient();

            string key = DateTime.Today.KGYLPasswordGenerator().GetMd5Hash();
            client.DefaultRequestHeaders.Add("KGYLKey", key);


            if (par1 == "All")
            {
                 url = $@"{endPoint}CUST/{userID}?par2={salesCat}";
            }
            else
            {
                 url = $@"{endPoint}CUST/{userID}?par2={salesCat}&par3={search}";
            }
            var response = await client.GetStringAsync(url);
            records = JsonConvert.DeserializeObject<List<CUST>>(response);


            CustListView.ItemsSource = string.Empty;
            CustListView.ItemsSource = records;


         }

        private void Btn_All_Clicked(object sender, EventArgs e)
        {
            Show_Custlist("");
        }

        private  void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as CUST;
			
			//string myCustID = myDetail.C_ID.Trim();
   //         Preferences.Set("CustID", myCustID);
   //         Preferences.Set("CustName", myDetail.C_NAME);


        }


        private void lbl_TEL_Tapped(object sender, EventArgs e)
        {
            //var teat = (sender as Label).Text;
            string cTEL = (sender as Label).Text;

            if (string.IsNullOrEmpty(cTEL))
            {
                DisplayAlert("Phone No", "Phone number does not Exist.", "Confirm");
                return;
            }

            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
            {
                phoneDialer.MakePhoneCall(cTEL);
            }
        }
    }
}