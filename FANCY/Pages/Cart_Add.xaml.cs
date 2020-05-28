using FANCY.Models;
using FANCY.ProjectClass;
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
    public partial class Cart_Add : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();
        
        private string custID = "";
        private string FullCustName = "";
        private string FullAddress = "";       
        private string custName = "";
        private string cPageFrom = Preferences.Get("PageFrom", "Customer");
        private string userID = Preferences.Get("UserID", "XXX");
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";
        private string barcode = "";
        private decimal oQty;
        private decimal iQty;
        private decimal iPrice;
        private decimal iMinprice;
        private decimal iCost = 0;
        private string qtyChanged = "";
        private string priceChanged = "";

        
        public Cart_Add()
        {
            InitializeComponent();  
            NavigationPage.SetHasBackButton(this, false);
            custID = Preferences.Get("CustID", "X");
            CheckCart();
            Clear_Barcode();
        }        
        public async void CheckCart()
        {
            CUST cust = await LocalDb.GetCUST(custID);
            FullCustName = cust.FullCustName;
            FullAddress = cust.FullAddress;
            custName = cust.C_NAME;
            txtQty.Text = "0";
            txtPrice.Text = "0";
            CustName.Text = FullCustName;
            CustAddress.Text = FullAddress;

            if (await LocalDb.TableExists("CART"))
            {
                Show_CartItem();
            }

            else
            {
                await DisplayAlert("STOP!", "Create Cart First!", "OK");
                return;
            }
        }

        private async void BarcodePressed(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                decimal custPrice = 0;

                barcode = txtBarcode.Text.Trim().ToUpper();

                string cSql = $@"select orcupr.*, trim(IT.I_preunit) AS o_iunit, trim(IT.I_desc1) AS o_idesc,
                                 IT.I_costc AS o_icostc, IT.I_pric AS o_iprice, IT.I_minpric AS o_iminpric,
                                 IT.I_onhand AS o_ionhand, trim(IT.I_size) AS o_isize
                                 FROM IT LEFT OUTER JOIN orcupr ON IT.I_id = orcupr.o_item
                                 WHERE IT.I_id = '{barcode}'";

                var rs = LocalDb.db.QueryAsync<orcupr>(cSql).Result.FirstOrDefault();
                decimal o_price;
                if (rs == null)
                {
                    var duration = TimeSpan.FromSeconds(3);
                    Vibration.Vibrate(duration);
                }
                else
                {
                    iCost = rs.o_icostc;
                    o_price = rs.o_oprice;
                    if (await CartItemFound())
                    {
                        Show_CartItem();
                        Clear_Barcode();
                    }
                    else
                    {
                        //qtyChanged = Preferences.Get("QtyChanged", "NO");
                        //priceChanged = Preferences.Get("PriceChanged", "NO");

                        //For Cart
                        if (o_price > 0) custPrice = rs.o_oprice;
                        else custPrice = rs.o_iprice;

                        if (custPrice == 0) await DisplayAlert("STOP!", "You need to enter the price", "OK");

                        if (priceChanged == "YES") custPrice = iPrice;
                        if (qtyChanged == "YES") iQty = oQty;
                        if (iQty == 0) iQty = 1;

                        List<CART> Cart = new List<CART>();

                        var c = new CART();

                        c.i_item = rs.o_item ?? barcode;
                        c.i_desc1 = rs.o_idesc;
                        c.i_qty = iQty;
                        c.i_unit = rs.o_unit ?? rs.o_iunit;
                        c.i_size = rs.o_isize ?? "";
                        c.i_onhand = rs.o_ionhand;
                        c.i_pri = custPrice;
                        c.i_damt = iQty * custPrice;
                        c.i_cost = iQty * rs.o_icostc;
                        c.i_profit = (iQty * custPrice) - (iQty * rs.o_icostc);
                        c.i_cust = custID.Trim();
                        c.i_user = userID.Trim();
                        c.i_comm = c.i_damt * (4 * (c.i_pri / c.i_damt) / 25);
                        c.i_cartdisplay1 = barcode + " : " +  rs.o_idesc + "-" + rs.o_isize;
                        c.i_cartdisplay2 = rs.o_unit + "  MinPrice: " + rs.o_iminpric.ToString("C") + "  Onhand: " + rs.o_ionhand.ToString();
                        c.i_oderdate = DateTime.Now;

                        Cart.Add(c);

                        await LocalDb.db.InsertAllAsync(Cart);
                        Show_CartItem();
                        Clear_Barcode();

                    }

                }
                
            }
            else await DisplayAlert("STOP!", "You need to enter Item first!", "OK");
        }

        private void Clear_Barcode()
        {
            txtBarcode.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtBarcode.Focus();
        }
        private async void Show_CartItem()
        {
            OrderListView.ItemsSource = string.Empty;

            var rs = await LocalDb.db.Table<CART>().Where(i => i.i_cust == custID).ToListAsync();
            var rs1 = rs.ToList();
            if (rs1.Count() > 0)
            {
                OrderListView.ItemsSource = rs1;
                decimal total = await LocalDb.GetTotalAmt(custID);
                decimal tqty = await LocalDb.GetTotalQty(custID);
                Lbl_Header_Total.Text = total.ToString();
                Lbl_Header_TotalQty.Text = tqty.ToString();             
            }
            else
            {
                return;
            }

            Show_Remarks();
            priceChanged = "NO";
            qtyChanged = "NO";
            iQty = 1;
            Clear_Barcode();
        }

        private async void Show_Remarks()
        {
            var rs = await LocalDb.db.Table<REMARK>().Where(c => c.r_cust == custID).FirstOrDefaultAsync();
            if (rs != null)
            {
                
                Remark1.Text = rs.r_rem1;
                Remark2.Text = rs.r_rem2;
                Remark3.Text = rs.r_rem3;
                Remark4.Text = rs.r_rem4;
                //Remark5.Text = rs.r_rem5;
                //Remark6.Text = rs.r_rem6;
            }
        }

        //private string GetOrderNo()
        //{
        //    string seed = DateTime.Now.ToYearMonthDayHourMinuteSecond() + Preferences.Get("CustomerID", "X");
        //    return seed.Trim();
        //}


        public async void Set_focus_Barcode()
        {
            await Task.Delay(1);
            txtBarcode.Text = string.Empty;
            txtBarcode.Focus();
        }
        private async void Btn_Delete_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int inum = int.Parse(button.CommandParameter.ToString());
            string cSQL = $@"delete from CART where i_num ={inum}";
            await LocalDb.db.ExecuteAsync(cSQL);
            Show_CartItem();

        }

        private async void OnQtyChangePressed(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            if (!string.IsNullOrEmpty(entry.Text))
            {
                int entryQty = int.Parse(entry.Text);
                var lvElement = (CART)((Entry)sender).BindingContext;
                decimal entryPrice = lvElement.i_pri;
                int inum = lvElement.i_num;
                string cSQL = $@"update CART set i_qty = {entryQty}, i_dAmt = {entryQty} * {entryPrice} where i_num ={inum}";
                await LocalDb.db.ExecuteAsync(cSQL);
                entry.Text = entryQty.ToString();
                Show_CartItem();
            }
            else await DisplayAlert("STOP!", "Enter The Quantity!", "OK");
        }

        private async void OnPriceChangePressed(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            if (!string.IsNullOrEmpty(entry.Text))
            {
                decimal entryPrice = decimal.Parse(entry.Text);
                var lvElement = (CART)((Entry)sender).BindingContext;
                decimal entryQty = lvElement.i_qty;
                int inum = lvElement.i_num;
                string cSQL = $@"update CART set i_pri = {entryPrice}, i_dAmt = {entryQty} * {entryPrice} where i_num ={inum}";
                await LocalDb.db.ExecuteAsync(cSQL);
                entry.Text = entryPrice.ToString();
                Show_CartItem();
            }
            else await DisplayAlert("STOP!", "Enter The Price!", "OK");
        }
        private async Task<Boolean> CartItemFound()
        {

            var ItemCount = await LocalDb.db.Table<CART>().Where(c => c.i_cust == custID && c.i_item == barcode).ToListAsync();
            if (ItemCount.Count() > 0)
            {
                var rs = LocalDb.db.Table<CART>().Where(c => c.i_cust == custID && c.i_item == barcode).FirstOrDefaultAsync();
                decimal itemPrice;
                string tempCust = rs.Result.i_cust;
                //Later itemQty is needed to bring from txtQty of Xaml with User's LOCK option. 
                decimal itemQty = rs.Result.i_qty + iQty;

                if (iPrice != 0 && iPrice != rs.Result.i_pri)
                {
                    itemPrice = iPrice;
                    return false;
                }
                else
                {
                    itemPrice = rs.Result.i_pri;
                    int inum = rs.Result.i_num;

                    string cSQL = $@"update CART set i_qty = {itemQty}, i_cost = {itemQty} * {iCost}, i_damt = {itemQty} * {itemPrice} where i_num ={inum}";
                    await LocalDb.db.ExecuteAsync(cSQL);
                    return true;
                }
            }
            else return false;

        }

        private void QtyChangePressed(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtQty.Text) && int.Parse(txtQty.Text) > 0)
            {
                qtyChanged = "YES";
                oQty = decimal.Parse(txtQty.Text.Trim());

            }
            txtPrice.Focus();

        }

        private void CheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            //chkLock.IsChecked = true;

        }

        private void PriceChangePressed(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtPrice.Text) && decimal.Parse(txtPrice.Text.Trim()) > 0)
            {
                priceChanged = "YES";
                DisplayAlert("Alert!", "The Price You Entered Is Lower than Minium Price : " + iMinprice.ToString("C"), "OK");
                iPrice = decimal.Parse(txtPrice.Text.Trim());
            }

            txtBarcode.Focus();

        }

        private async void BtnCancelOrder_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("STOP!", "Sure To Cancel This Order?", "YES", "NO");
            if (answer)
            {
                string cSQL = $@"DELETE FROM CART WHERE i_cust = '" + custID + "'";
                await LocalDb.db.ExecuteAsync(cSQL);
                string sSQL = $@"DELETE FROM REMARK WHERE r_cust = '" + custID + "'";
                await LocalDb.db.ExecuteAsync(sSQL);
                Page page1 = new Pages.Cust();
                await Navigation.PushAsync(page1, false);
            }

        }

        private async void RemarkPressed(object sender, EventArgs e)
        {

            string eSwitch = (((Entry)sender).BindingContext).ToString();
            string rm = "";
            string strSql = "";

            switch (eSwitch)
            {
                case "Remark1":
                    
                    rm = string.IsNullOrEmpty(Remark1.Text) ? " " : Remark1.Text;
                    string cSQL1 = $@"update REMARK set r_rem1 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                    strSql = cSQL1;
                    break;
                case "Remark2":
                    rm = string.IsNullOrEmpty(Remark2.Text) ? " " : Remark2.Text;
                    string cSQL2 = $@"update REMARK set r_rem2 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                    strSql = cSQL2;
                    break;
                case "Remark3":
                    rm = string.IsNullOrEmpty(Remark3.Text) ? " " : Remark3.Text;
                    string cSQL3 = $@"update REMARK set r_rem3 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                    strSql = cSQL3;
                    break;
                case "Remark4":
                    rm = string.IsNullOrEmpty(Remark4.Text) ? " " : Remark4.Text;
                    string cSQL4 = $@"update REMARK set r_rem4 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                    strSql = cSQL4;
                    break;
                //case "Remark5":
                //    rm = string.IsNullOrEmpty(Remark5.Text) ? " " : Remark5.Text;
                //    string cSQL5 = $@"update REMARK set r_rem5 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                //    strSql = cSQL5;
                //    break;
                //case "Remark6":
                //    rm = string.IsNullOrEmpty(Remark6.Text) ? " " : Remark6.Text;
                //    string cSQL6 = $@"update REMARK set r_rem6 = '" + rm + "'" + " where r_cust = '" + custID + "'";
                //    strSql = cSQL6;
                //    break;
            }

            await LocalDb.db.ExecuteAsync(strSql);

        }

        private async void BtnSubmitOrder_Clicked(object sender, EventArgs e)
        {
            activity_indicator.IsVisible = true;
            activity_indicator.IsRunning = true;

            var answer = await DisplayAlert("SUBMIT ORDER!", "Do You Want To Submit This Order Now?", "YES", "NO");
            if (answer)
            {
                string ret_value = await LocalDb.SubmitOrder(custID, userID);
                if (ret_value == "Error")
                {
                    await DisplayAlert("ALERT !", "There is Sytem Error, Order Submit is Not Successful @ @", "OK");
                }

                OrderListView.ItemsSource = string.Empty;
                activity_indicator.IsRunning = false;
                activity_indicator.IsVisible = false;

                await DisplayAlert("SUBMIT SUCCESS !", ret_value, "OK");

                if (cPageFrom == "Menu")
                {
                    await Navigation.PushAsync(new Menu());
                }
                else if (cPageFrom == "Customer")
                {
                    await Navigation.PushAsync(new Cust());
                }
                else if (cPageFrom == "CartList")
                {
                    await Navigation.PushAsync(new CartList());
                }
                else
                {
                    await Navigation.PushAsync(new Cust());
                }

            }

        }

        private async void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            int id = Convert.ToInt32(mi.CommandParameter);
            //string id = mi.CommandParameter.ToString();
            try
            {
                string cSQL = $@"DELETE FROM CART WHERE i_num = '{id}'";
                await LocalDb.db.ExecuteAsync(cSQL);
                
                //await DisplayAlert("Alert!", "Order Item Deleted", "Ok");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "Ok");
            }

            Show_CartItem();
        }

        private void Show_ItemPrice(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                decimal custPrice = 0;

                barcode = txtBarcode.Text.Trim().ToUpper();

                string cSql = $@"select orcupr.*, trim(IT.I_preunit) AS o_iunit, trim(IT.I_desc1) AS o_idesc, IT.I_costc AS o_icostc, IT.I_pric AS o_iprice, IT.I_minpric AS o_iminpric,
                                 IT.I_onhand AS o_ionhand, trim(IT.I_size) AS o_isize
                                 FROM IT LEFT OUTER JOIN orcupr ON IT.I_id = orcupr.o_item
                                 WHERE IT.I_id = '{barcode}'";

                var rs = LocalDb.db.QueryAsync<orcupr>(cSql).Result.FirstOrDefault();

                decimal o_price;

                if (rs == null)
                {
                    var duration = TimeSpan.FromSeconds(3);
                    Vibration.Vibrate(duration);
                }
                else
                {
                    o_price = rs.o_oprice;
                    iMinprice = rs.o_iminpric;

                    if (o_price > 0) custPrice = rs.o_oprice;
                    else custPrice = rs.o_iprice;

                    txtPrice.Text = custPrice.ToString();
                    
                }
            }
        }
    }
}