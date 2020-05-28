using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FANCY.ProjectClass;
using FANCY.Models;
using System.Linq;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();
        private bool isCartExist = false;
		 
        public Menu()
        {
            InitializeComponent();

            this.Title = "EZ Sales Manager";
            NavigationPage.SetHasBackButton(this, false);
			
        }

        private async void BtnUpdateData_Clicked(object sender, EventArgs e)
        {            
            await Shell.Current.GoToAsync("DataManagement");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            lbl_Date.Text = DateTime.Today.ToLongDateString();
            lbl_Company.Text = "NY Fancy Natural Foods \n";
			isCartExist = await LocalDb.TableExists("CART");
            if (isCartExist == true)
            { 
                GetOrderList();
            }
        }

        private void GetOrderList()
        {
            List<orderlist> oRList =  new List<orderlist>();

            string sql = $@"SELECT i_cust as t_cust, SUM(i_damt) as t_damt, MAX(C_NAME) as t_name, MAX(C_ADDRESS) as t_address,
                            MAX(C_CITY) as t_city, MAX(C_TEL) as t_tel, MAX(i_user) as t_suser FROM CART LEFT OUTER JOIN CUST ON cart.i_cust = CUST.C_ID
                            GROUP BY cart.i_cust 
                            ORDER BY i_cust";

            var rd = LocalDb.db.QueryAsync<tmp_orderlist>(sql).Result.ToList();

            if (rd !=null || rd.Count() !=0)
            { 
	            foreach (tmp_orderlist tmp in rd)
	            {
	                orderlist h = new orderlist();
	
	                h.ol_cust = tmp.t_cust;
	                h.ol_custn = tmp.t_name;
	                h.ol_suser = tmp.t_suser;
	                h.ol_custadd = $@"{tmp.t_address.Trim()} {tmp.t_city.Trim()} {tmp.t_tel.Trim()}";
	                h.ol_damt = tmp.t_damt;
	
	                oRList.Add(h);
	            }
            
	            ItemOrderList.ItemsSource = string.Empty;
	            ItemOrderList.ItemsSource = oRList;
			}
        }

        private async void Btn_Order_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.DailyList();
            await Navigation.PushAsync(page, false);
        }

        private async void Btn_AR_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.CustomerBal();
            await Navigation.PushAsync(page, false);

        }
        private async void Btn_Cust_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Customer();
            await Navigation.PushAsync(page, false);

        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Gate();
            await Navigation.PushAsync(page, false);

        }

        private async void OrderList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as orderlist;
            string myCustID = myDetail.ol_cust.Trim();
            string myCustName = myDetail.ol_custn;

            Preferences.Set("CustID", myCustID);
            Preferences.Set("CustName", myCustName);

            Preferences.Set("PageFrom", "Menu");

            await Navigation.PushAsync(new Cart_New());
        }

        private async void Btn_submit_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Grid grd1 = (Grid)button.Parent;
            ActivityIndicator a = (ActivityIndicator)grd1.FindByName("activity_indicator");


            var lvElement = (orderlist)((Button)sender).BindingContext;
            string custID = lvElement.ol_cust;
            string userID = lvElement.ol_suser;
            var answer = await DisplayAlert("SUBMIT ORDER!", "Do You Want To Submit This Order Now?", "YES", "NO");
            if (answer)
            {
                a.IsRunning = true;

                string ret_value = await LocalDb.SubmitOrder(custID, userID);
                if (ret_value == "Error")
                {
                    await DisplayAlert("ALERT !", "There is Sytem Error, Order Submit is Not Successful @ @", "OK");
                }
                await DisplayAlert("SUBMIT SUCCESS !", ret_value, "OK");
                GetOrderList();

                a.IsRunning = false;
            }
        }
    }
}