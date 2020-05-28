using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartList : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();
        private bool isCartExist = false;

        public CartList()
        {
            InitializeComponent();

            this.Title = "Outstanding Orders";
            NavigationPage.SetHasBackButton(this, false);

        }

        private async void BtnUpdateData_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("DataManagement");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //lbl_Date.Text = DateTime.Today.ToLongDateString();
            //lbl_Company.Text = "NY Fancy Natural Foods \n";
            isCartExist = await LocalDb.TableExists("CART");
            if (isCartExist == true)
            {
                GetOrderList();
            }
        }

        private void GetOrderList()
        {
            List<orderlist> oRList = new List<orderlist>();

            string sql = $@"SELECT i_cust as t_cust, SUM(i_damt) as t_damt, MAX(C_NAME) as t_name, MAX(C_ADDRESS) as t_address,
                            MAX(C_CITY) as t_city, MAX(C_TEL) as t_tel, MAX(i_user) as t_suser FROM CART LEFT OUTER JOIN CUST ON cart.i_cust = CUST.C_ID
                            GROUP BY cart.i_cust 
                            ORDER BY i_cust";

            var rd = LocalDb.db.QueryAsync<tmp_orderlist>(sql).Result.ToList();

            if (rd != null || rd.Count() != 0)
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

        

        private async void OrderList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var myDetail = e.Item as orderlist;
            string myCustID = myDetail.ol_cust.Trim();
            string myCustName = myDetail.ol_custn;

            Preferences.Set("CustID", myCustID);
            Preferences.Set("CustName", myCustName);

            Preferences.Set("PageFrom", "CartList");

            await Navigation.PushAsync(new Cart_New());

            //await Navigation.PopAsync();
            //await Navigation.PushAsync(new CartList());
        }

        
    }
}