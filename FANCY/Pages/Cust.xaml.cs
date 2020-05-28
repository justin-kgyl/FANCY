using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cust : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();
        private bool isCartExisted;
        private bool isCustExisted;
        public Cust()
        {
            InitializeComponent();
            Title = "Customer List";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            isCustExisted = await LocalDb.TableExists("CUST");
            isCartExisted = await LocalDb.TableExists("CART");
            if (isCartExisted == true && isCustExisted == true)
            {
                Show_Custlist("All");
            }
            else await DisplayAlert("ALERT!", "You Need To Create Cart And Download Data From Server First @ @", "OK");
        }

        private void ShowCustomer_Clicked(object sender, System.EventArgs e)
        {
            Show_Custlist("");
        }

        private async void Show_Custlist(string par1)
        {
            string sql = "";
            string search = "";

            if (par1 == "All")
            {
                sql = @"SELECT * 
                        FROM CUST 
                        ORDER BY C_NAME";
            }
            else
            {
                search = txtCustName.Text.ToUpper().Trim();

                if (string.IsNullOrWhiteSpace(search) == false)
                {

                    sql = $@"SELECT * 
                             FROM CUST 
                             WHERE C_NAME LIKE '%{search}%'
                             ORDER BY C_NAME";
                }
                else
                {
                    await DisplayAlert("Alert!", "Enter Customer Name", "OK");
                    return;
                }

            }

            var list = await LocalDb.db.QueryAsync<CUST>(sql);

            if (list.Count == 0)
            {
                await DisplayAlert("Alert!", "No Such Customer Exist!", "OK");
                CustListView.ItemsSource = string.Empty;
                return;
            }
            else
            {
                CustListView.ItemsSource = list;
            }

        }

        private void Btn_All_Clicked(object sender, EventArgs e)
        {
            Show_Custlist("");
        }

        private async void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as CUST;
			string myCustID = myDetail.C_ID.Trim();
            string myCustName = myDetail.C_NAME;
            Preferences.Set("CustID", myCustID);
            Preferences.Set("CustName", myCustName);

            isCartExisted = await LocalDb.TableExists("CART");
            if (isCartExisted == true)
            {

                string sSQL = $"SELECT * FROM REMARK WHERE r_cust = '{myCustID}'";

                var rs = LocalDb.db.QueryAsync<REMARK>(sSQL).Result.FirstOrDefault();

                if (rs == null)
                {
                    REMARK remark = new REMARK
                    {
                        r_cust = myCustID,
                        r_rem1 = "",
                        r_rem2 = "",
                        r_rem3 = "",
                        r_rem4 = "",
                        r_rem5 = "",
                        r_rem6 = ""
                    };
                    await LocalDb.db.InsertAsync(remark);
                }
                await Navigation.PushAsync(new CustomerInfo(myDetail.C_ID));
            }
            else await DisplayAlert("ALERT!", "You need to creat CART First @ @", "OK");
        }

        public async void CheckRemark()
        {
            if (!await LocalDb.TableExists("REMARK"))
            {
                await DisplayAlert("STOP!", "Create Remark First!", "OK");
                return;
            }
        }

        private void lbl_TEL_Tapped(object sender, EventArgs e)
        {
            string cTEL = (sender as Label).Text;

            try
            {
                PhoneDialer.Open(cTEL);
            }
            catch (Exception ex)
            {
                DisplayAlert("Unable to make Call", ex.Message, "Ok");
            }

        }
    }
}