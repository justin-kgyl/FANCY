using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataManagement : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();

        private string saleID = Preferences.Get("UserID", "XXXX");
        private string salesCat = Preferences.Get("SalesCat", "SLS");

        public DataManagement()
        {
            InitializeComponent();

            Title = "Data Managerment";
            Shell.SetTabBarIsVisible(this, false);

            Lbl_UserName.Text = Preferences.Get("UserName", "");

            ShowUpdateDate();
        }

        private async void BtnCreateCart_Clicked(object sender, EventArgs e)                // Cart & Remmark
        {
            if (await LocalDb.TableExists("CART"))
            {
                bool boolResult = await LocalDb.IsCartEmpty();

                if (boolResult)

                {
                    await LocalDb.db.DropTableAsync<CART>();
                    await LocalDb.db.CreateTableAsync<CART>();
                    await DisplayAlert("CART", "A Cart is Created Successfully", "OK");
                }
                else
                {
                    bool answer = await DisplayAlert("STOP!", "There are unsubmitted order(s) in cart, still want to create cart?", "Yes", "No");
                    Debug.WriteLine("Answer: " + answer);
                    if (answer)
                    {
                        await LocalDb.db.DropTableAsync<CART>();
                        await LocalDb.db.CreateTableAsync<CART>();
                        await DisplayAlert("CART", "A Cart is Created Successfully", "OK");
                    }
                }
            }
            else
            {
                await LocalDb.db.CreateTableAsync<CART>();
                await DisplayAlert("CART", "The CART is Created Successfully", "OK");

            }

            if (await LocalDb.TableExists("REMARK"))
            {
                bool boolResult = await LocalDb.IsRemarkEmpty();

                if (boolResult)

                {
                    await LocalDb.db.DropTableAsync<REMARK>();
                    await LocalDb.db.CreateTableAsync<REMARK>();
                    await DisplayAlert("REMARK", "A REMARK is Created Successfully", "OK");
                }
                else
                {
                    bool answer = await DisplayAlert("STOP!", "There are unsubmitted order(s) in REMARK, still want to create REMARK?", "Yes", "No");
                    Debug.WriteLine("Answer: " + answer);
                    if (answer)
                    {
                        await LocalDb.db.DropTableAsync<REMARK>();
                        await LocalDb.db.CreateTableAsync<REMARK>();
                        await DisplayAlert("REMARK", "A REMARK is Created Successfully", "OK");
                    }
                }
            }
            else
            {
                await LocalDb.db.CreateTableAsync<REMARK>();
                await DisplayAlert("REMARK", "The REMARK is Created Successfully", "OK");
            }
        }

        private async void BtnUpdateAllData_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Internet Connection", "ok");
                return;
            }

            bool itemClassSelected = SWT_itemClass.IsToggled;
            bool itemSelected = SWT_item.IsToggled;
            bool customerSelected = SWT_customer.IsToggled;
            bool priceSelected = SWT_price.IsToggled;
            bool arSelected = SWT_ar.IsToggled;
                        
            activity_indicator.IsRunning = true;
            activity_indicator.IsVisible = true;
            BtnUpdateAllData.IsEnabled = false;

            if (itemClassSelected)
            {
                var itemCount = await LocalDb.CreateAndUpdateTable<ITG>("ITG");
                LBL_itemclass.Text = itemCount.ToString() + " Record(s)";
                SWT_itemClass.IsToggled = false;
                Preferences.Set("ItemClassDate", DateTime.Now);
            }

            if (itemSelected)
            {
                var itemCount = await LocalDb.CreateAndUpdateTable<IT>("IT"); 
                LBL_item.Text = itemCount.ToString() + " Record(s)";
                SWT_item.IsToggled = false;
                Preferences.Set("ItemDate", DateTime.Now);
            }

            if (customerSelected)
            {
                var customerCount = await LocalDb.CreateAndUpdateTable<CUST>("CUST", saleID);
                LBL_customer.Text = customerCount.ToString() + " Record(s)";
                SWT_customer.IsToggled = false;
                Preferences.Set("CustomerDate", DateTime.Now);
            }

            if (priceSelected)
            {
                var orcuprCount = await LocalDb.CreateAndUpdateTable<orcupr>("ORCUPR", saleID);
                LBL_price.Text = orcuprCount.ToString() + " Record(s)";
                SWT_price.IsToggled = false;
                Preferences.Set("PriceDate", DateTime.Now);
            }

            if (arSelected)
            {
                var invoiceCount = await LocalDb.CreateAndUpdateTable<inv>("INV", saleID);
                //LBL_ar.Text = invoiceCount.ToString() + " Record(s)";
                //SWT_ar.IsToggled = false;
                //Preferences.Set("ARDate", DateTime.Now);

                invoiceCount = await LocalDb.CreateAndUpdateTable<invdet>("INVDET", saleID);
                LBL_ar.Text = invoiceCount.ToString() + " Record(s)";
                SWT_ar.IsToggled = false;
                Preferences.Set("ARDate", DateTime.Now);
            }

            activity_indicator.IsRunning = false;
            activity_indicator.IsVisible = false;
            BtnUpdateAllData.IsEnabled = true;
            SWT_All.IsToggled = false;
        }        

        private async void BtnCustomer_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Customer();
            await Navigation.PushAsync(page, false);
        }

        private void Btn_AllOrder_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn_AllItem_Clicked(object sender, EventArgs e)
        {

        }
        private void Btn_Order_Clicked(object sender, EventArgs e)
        {

        }
        private async void Btn_Item_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Item();
            await Navigation.PushAsync(page, false);
        }

        protected void SWT_All_Toggled(object sender, ToggledEventArgs e)
        {
            if (SWT_All.IsToggled)
            {
                SWT_itemClass.IsToggled = true;
                SWT_item.IsToggled = true;
                SWT_customer.IsToggled = true;
                SWT_price.IsToggled = true;
                SWT_ar.IsToggled = true;
            }
            else
            {
                SWT_itemClass.IsToggled = false;
                SWT_item.IsToggled = false;
                SWT_customer.IsToggled = false;
                SWT_price.IsToggled = false;
                SWT_ar.IsToggled = false;
            }
        }

        protected void ShowUpdateDate()
        {
            DateTime DefaulDateTime = new DateTime(2001, 1, 1);

            var itemClassDate = Preferences.Get("ItemClassDate", DefaulDateTime);
            var itemDate = Preferences.Get("ItemDate", DefaulDateTime);
            var customerDate = Preferences.Get("CustomerDate", DefaulDateTime); 
            var priceDate = Preferences.Get("PriceDate", DefaulDateTime);
            var arDate = Preferences.Get("ARDate", DefaulDateTime);

            if (itemClassDate != DefaulDateTime) LBL_itemclass.Text = "Completed " + itemClassDate.ToShortDateString();
            if (itemDate != DefaulDateTime) LBL_item.Text = "Completed " + itemDate.ToShortDateString();
            if (customerDate != DefaulDateTime) LBL_customer.Text = "Completed " + customerDate.ToShortDateString();
            if (priceDate != DefaulDateTime) LBL_price.Text = "Completed " + priceDate.ToShortDateString();
            if (arDate != DefaulDateTime) LBL_ar.Text = "Completed " + arDate.ToShortDateString();
        }
    }
}