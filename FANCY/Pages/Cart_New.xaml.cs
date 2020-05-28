using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cart_New : ContentPage
    {
        private KGYLDatabase LocalDb = new KGYLDatabase();      
        List<orddet> orddets = new List<orddet>();

        private string _UserID = Preferences.Get("UserID", "XXXX");
        private string _CustID = Preferences.Get("CustID", "XXXX");
        private string endPoint = Preferences.Get("ServerIpAddress", "XXXX") + "/MOBILE/";
        private string newPrice = "";
        private string barcode = "";
        private decimal iQty = 0;
        private decimal iPrice = 0;

        Entry E1 = new Entry();
        Entry E2 = new Entry();
        Label L1 = new Label();

        public Cart_New()
        {
            InitializeComponent();
            Cust_Info();
            Show_Cart();
            slo_ItemListView.IsVisible = false;
            slo_btn_cart.IsVisible = false;
        }

        public void Cust_Info()
        {
            string _CustID = Preferences.Get("CustID", "XXXX");

            string sql;

            sql = "";
            sql += "SELECT * ";
            sql += "FROM CUST ";
            sql += "WHERE C_ID = '" + _CustID + "'";

            var list = LocalDb.db.QueryAsync<CUST>(sql).Result.ToList();

            lbl_FullAddress.Text = list[0].FullAddress.ToString();
            lbl_NAME.Text = list[0].C_NAME.ToString();
            //lbl_REGION.Text = list[0].C_REGION.ToString();
            lbl_TERM.Text = list[0].C_TERM.ToString();
            //lbl_SALESREP.Text = list[0].C_SALESREP.ToString();
            lbl_BALANCE.Text = list[0].C_BALANCE.ToString("C");
            //lbl_ATTN.Text = list[0].C_ATTN.ToString();

        }

        private void txt_itemgroup_Completed(object sender, EventArgs e)
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var height = mainDisplayInfo.Height;
            //slo_ItemListView.HeightRequest = height;


            orddets.Clear();

            string itemgroup = txt_itemgroup.Text;

            if (!string.IsNullOrEmpty(itemgroup))
            {
                itemgroup = txt_itemgroup.Text.ToUpper();
            }

            if (string.IsNullOrEmpty(itemgroup))
            {
                ItemListView.ItemsSource = string.Empty;
                slo_ItemListView.IsVisible = false;
                slo_btn_cart.IsVisible = false;

                Show_Cart();
                return;
            }

            string cID = Preferences.Get("CustID", "XXXX");

            if (itemgroup.Length != 2)
            {
                DisplayAlert("Alert", "Item Group must be 2 characters", "Cancel");
                return;
            }


            string cSQL = $@"SELECT IT.I_ID as io_id, IT.I_DESC1 as io_desc1, IT.I_CLASS as io_class, IT.I_SIZE as io_size, IT.I_PRIC as io_pric, IT.I_ONHAND as io_onhand, IT.I_MINPRIC as io_minpric, IT.I_COSTC as io_costc, IT.I_PREUNIT as io_preunit, * FROM IT ";
            cSQL = cSQL + $@"left outer join (select orcupr.o_cust as io_cust, orcupr.o_item as io_item, orcupr.o_unit as io_unit, orcupr.o_oprice as io_price, orcupr.o_lastd as io_lastd ";
            cSQL = cSQL + $@"from orcupr where o_cust = '{cID}') as t_det on IT.I_ID = t_det.io_item where UPPER(IT.I_CLASS) = '{itemgroup}' AND IT.I_ACTIVE = 'Y' order by IT.I_DESC1";


            var list = LocalDb.db.QueryAsync<it_orcupr>(cSQL).Result.ToList();

            if (list.Count == 0)
            {
                ItemListView.ItemsSource = string.Empty;
                DisplayAlert("Info", "No Item Found", "Cancel");
                slo_ItemListView.IsVisible = false;
                slo_btn_cart.IsVisible = false;

                Show_Cart();
                return;
            }
            else
            {
                foreach (it_orcupr dr in list)
                {
                    orddet h = new orddet();
                    h.od_id = dr.io_id;
                    h.od_desc1 = dr.io_desc1.Trim();
                    h.od_desc2 = dr.io_desc1.Trim() + " (" + dr.io_size.Trim() + ")";
                    h.od_minpri = dr.io_minpric;
                    h.od_onhand = dr.io_onhand;
                    h.od_size = dr.io_size;
                    h.od_qty = 0;
                    h.od_cost = dr.io_costc;
                    h.od_cust = cID.Trim();
                    h.od_user = Preferences.Get("UserID", "XXXX");

                    decimal cPRICE = dr.io_pric;
                    decimal? cOLDPRICE = dr.io_price;
                    if (cOLDPRICE != null)
                    {
                        h.od_pri = decimal.Parse(cOLDPRICE.ToString());
                    }
                    else
                    {
                        h.od_pri = cPRICE;
                    }

                    if (dr.io_onhand < 0)
                    {
                        h.od_color = "Red";
                    }
                    else
                    {
                        h.od_color = "Black";
                    }

                    h.od_damt = 0m;
                    orddets.Add(h);
                }

                ItemListView.ItemsSource = string.Empty;
                ItemListView.ItemsSource = orddets;
            }

            slo_ItemListView.IsVisible = true;
            slo_btn_cart.IsVisible = true;

            if (swt_show_cart.IsToggled)
            {
                SL_CartListView.IsVisible = true;
            }
            else
            {
                SL_CartListView.IsVisible = false;
            }
            

            //DisplayAlert("OK", list.Count.ToString(), "Cancel");
        }

        private void btn_Plus_Clicked(object sender, EventArgs e)
        {
            var duration = TimeSpan.FromSeconds(0.1);
            Vibration.Vibrate(duration);

            Button button = (Button)sender;

            Grid ParentGrid = (Grid)button.Parent;

            Entry txtpri = (Entry)ParentGrid.FindByName("txt_pri");
            Entry txtqty = (Entry)ParentGrid.FindByName("txt_qty");
            Label lbldamt = (Label)ParentGrid.FindByName("lbl_damt");

            decimal ordPri = decimal.Parse(txtpri.Text);
            int ordQty = int.Parse(txtqty.Text);

            ordQty = ordQty + 1;

            txtqty.Text = ordQty.ToString();
            lbldamt.Text = (ordPri * ordQty).ToString("C2");

        }



        private void btn_Minus_Clicked(object sender, EventArgs e)
        {
            var duration = TimeSpan.FromSeconds(0.1);
            Vibration.Vibrate(duration);

            Button button = (Button)sender;

            Grid ParentGrid = (Grid)button.Parent;

            Entry txtpri = (Entry)ParentGrid.FindByName("txt_pri");
            Entry txtqty = (Entry)ParentGrid.FindByName("txt_qty");
            Label lbldamt = (Label)ParentGrid.FindByName("lbl_damt");

            decimal ordPri = decimal.Parse(txtpri.Text);
            int ordQty = int.Parse(txtqty.Text);

            if (ordQty > 0)
            {
                ordQty = ordQty - 1;
            }


            txtqty.Text = ordQty.ToString();
            lbldamt.Text = (ordPri * ordQty).ToString("C2");
        }



        private async void btn_cart_Clicked(object sender, EventArgs e)
        {
            CheckCart();

            string result_value = await DisplayPromptAsync("Price!", "Enter price?", initialValue: "", cancel: "", keyboard: Keyboard.Numeric);


            if (!string.IsNullOrEmpty(result_value))
            {
                newPrice = result_value.Trim();
            }

            List<CART> listCart = new List<CART>();
            listCart.Clear();

            foreach (orddet h in orddets)
            {
                if (h.od_qty != 0)
                {
                    CART cart = new CART();
                    cart.i_cartdisplay1= h.od_id + " : " + h.od_desc1 + "-" + h.od_size;
                    cart.i_cartdisplay2 = h.od_unit + "  MinPrice: " + h.od_minpri.ToString("C") + "  Onhand: " + h.od_onhand.ToString();
                    cart.i_item = h.od_id.Trim();
                    cart.i_desc1 = h.od_desc1.Trim();
                    cart.i_qty = h.od_qty;
                    cart.i_unit = "BOX";
                    cart.i_size = h.od_size.Trim();
                    if (newPrice == "")
                    {
                        iPrice = 0;
                        cart.i_pri = h.od_pri;
                        barcode = cart.i_item.Trim();
                        iQty = cart.i_qty;
                        if (await CartItemFound(barcode, iQty)) goto Found;


                    }
                    else
                    {
                        cart.i_pri = decimal.Parse(newPrice);

                    }
                    cart.i_user = _UserID;
                    decimal ncost = 0;
                    if (h.od_cost != null)
                    {
                        ncost = decimal.Parse(h.od_cost.ToString());
                    }
                    cart.i_cost = ncost;

                    decimal ndamt = 0;
                    ndamt = cart.i_pri * h.od_qty;

                    cart.i_damt = ndamt;


                    decimal ntotalCost = ncost * h.od_qty;
                    decimal nprofit = ndamt - ntotalCost;
                    cart.i_profit = nprofit;

                    decimal nrate = 0;
                    if (ndamt != 0)
                    {
                        nrate = (nprofit / ndamt);
                    }
                    decimal ncomm = ndamt * 4 * nrate / 25;

                    cart.i_comm = ncomm;
                    cart.i_cust = h.od_cust;
                    cart.i_oderdate = DateTime.Now;

                    listCart.Add(cart);
                }

            }

            await LocalDb.db.InsertAllAsync(listCart);
           
            Found:
            
            Show_Cart();

            orddets.Clear();
            ItemListView.ItemsSource = string.Empty;
            ItemListView.ItemsSource = orddets;

            slo_ItemListView.IsVisible = false;

            if (swt_show_cart.IsToggled == true)
            {
                SL_CartListView.IsVisible = true;
            }
            else
            {
                SL_CartListView.IsVisible = false;
            }
            
            slo_btn_cart.IsVisible = false;


            Clear();
        }

        private void Clear()
        {
            txt_itemgroup.Text = "";
            //txt_itemgroup.Focus();
        }

        public async void CheckCart()
        {
            if (!await LocalDb.TableExists("CART"))
            {
                await DisplayAlert("STOP!", "Create Cart First!", "OK");
                return;
            }
        }

        private async void Show_Cart()
        {

            var rs = await LocalDb.db.Table<CART>().Where(i => i.i_cust == _CustID).OrderByDescending(i => i.i_num).ToListAsync();
            var rs1 = rs.ToList();
            if (rs1.Count() > 0)
            {

                CartListView.ItemsSource = rs1;
                decimal total_damt = await LocalDb.GetTotalAmt(_CustID);
                decimal total_qty = await LocalDb.GetTotalQty(_CustID);

                lbl_total_damt.Text = total_damt.ToString("C");
                lbl_total_qry.Text = total_qty.ToString();

                Show_Remarks();
                //await DisplayAlert("S", Lbl_Header_Total.Text, "OK");              
            }
            else
            {

                SL_CartListView.IsVisible = false;
                CartListView.ItemsSource = string.Empty;

                return;
            }
        }

        private async void Show_Remarks()
        {
            

            var rs = await LocalDb.db.Table<REMARK>().Where(c => c.r_cust == _CustID).FirstOrDefaultAsync();
            if (rs != null)
            {
                txt_rem1.Text = rs.r_rem1;
                txt_rem2.Text = rs.r_rem2;
                txt_rem3.Text = rs.r_rem3;
                txt_rem4.Text = rs.r_rem4;
            }
        }

        private async Task<Boolean> CartItemFound(string barcode, decimal iQty)
        {

            var ItemCount = await LocalDb.db.Table<CART>().Where(c => c.i_cust == _CustID && c.i_item == barcode).ToListAsync();
            if (ItemCount.Count() > 0)
            {
                var rs = LocalDb.db.Table<CART>().Where(c => c.i_cust == _CustID && c.i_item == barcode).FirstOrDefaultAsync();
                decimal itemPrice;
                decimal iCost = rs.Result.i_cost;
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

        private void swt_show_cart_Toggled(object sender, ToggledEventArgs e)
        {
            //DisplayAlert("Ok", swt_show_cart.IsToggled.ToString(), "OK");
            if (swt_show_cart.IsToggled)
            {
                SL_CartListView.IsVisible = true;
                
            }
            else
            {
                SL_CartListView.IsVisible = false;
                
            }
        }

        private async void btn_remove_item_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            var lvElement = (CART)((Button)sender).BindingContext;

            int nNum = lvElement.i_num;
            string cItem = lvElement.i_item.Trim();
            string cDesc = lvElement.i_desc1.Trim();

            string cQuestion = $@"Would you like to remove [{cItem}] item ?";

            
            var duration = TimeSpan.FromSeconds(0.1);
            Vibration.Vibrate(duration);

            bool answer = await DisplayAlert(cDesc, cQuestion, "Yes", "No");
            if (answer)
            {
                string sql = $@"DELETE FROM CART WHERE i_num = {nNum}";
                await LocalDb.db.ExecuteAsync(sql);

                Show_Cart();
            }
        }

        private async void btn_cancel_Clicked(object sender, EventArgs e)
        {
            string _CustID = Preferences.Get("CustID", "XXXX");

            bool answer = await DisplayAlert("Question?", "Would you like to cancel order?", "Yes", "No");

            if (answer)
            {
                string sql = $@"DELETE FROM CART WHERE i_cust = '{_CustID}'";
                await LocalDb.db.ExecuteAsync(sql);

                sql = $@"DELETE FROM REMARK WHERE r_cust = '{_CustID}'";
                await LocalDb.db.ExecuteAsync(sql);

                Show_Cart();

                SL_CartListView.IsVisible = false;
            }
            
            

            //string cPageFrom = Preferences.Get("PageFrom", "Customer");


            //if (cPageFrom == "Menu")
            //{
            //    await Navigation.PushAsync(new Menu());
            //}
            //else if (cPageFrom == "Customer")
            //{
            //    await Navigation.PushAsync(new Cust());
            //}
            //else if (cPageFrom == "CartList")
            //{
            //    await Navigation.PushAsync(new CartList());
            //}
            //else
            //{
            //    await Navigation.PushAsync(new Cust());
            //}
        }

        private async void btn_submit_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Alert", "Please check Internet Connection", "ok");
                return;
            }

            activity_indicator.IsRunning = true;
            //activity_indicator.IsVisible = true;

            string _CustID = Preferences.Get("CustID", "XXXX");
            string _UserID = Preferences.Get("UserID", "XXXX");

            var answer = await DisplayAlert("SUBMIT ORDER!", "Do You Want To Submit This Order Now?", "YES", "NO");
            if (answer)
            {
                btn_submit.IsEnabled = false;

                string ret_value = await LocalDb.SubmitOrder(_CustID, _UserID);
                if (ret_value == "Error")
                {
                    await DisplayAlert("ALERT !", "There is Sytem Error, Order Submit is Not Successful @ @", "OK");
                }

                await DisplayAlert("SUBMIT SUCCESS !", ret_value, "OK");
            }

            Show_Cart();
            btn_submit.IsEnabled = true;

            activity_indicator.IsRunning = false;

            string cPageFrom = Preferences.Get("PageFrom", "Customer");


            await Navigation.PushAsync(new Cust());
        }


        private void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            int cnum = Convert.ToInt32(mi.CommandParameter);
            //string id = mi.CommandParameter.ToString();
            try
            {
                string sql = $@"DELETE FROM CART WHERE i_num = {cnum}";
                LocalDb.db.ExecuteAsync(sql);
                Show_Cart();
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", ex.Message, "Ok");
                Show_Cart();
            }
        }

        private void btn_Hist_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Grid G1 = (Grid)button.Parent;


            E1 = (Entry)G1.FindByName("txt_pri");
            E2 = (Entry)G1.FindByName("txt_qty");
            L1 = (Label)G1.FindByName("lbl_damt");
            

            var lvElement = (orddet)((Button)sender).BindingContext;

            string _citem = lvElement.od_id;

            
    
            string _CustID = Preferences.Get("CustID", "XXXX");
            string _UserID = Preferences.Get("UserID", "XXXX");


            string sql = $@"SELECT * FROM invdet 
                            WHERE i_bill = '{_CustID}' AND i_item = '{_citem}'
                            ORDER BY i_id DESC LIMIT 3";

            var list = LocalDb.db.QueryAsync<invdet>(sql).Result.ToList();

            List<invdet> invdetList = new List<invdet>();

            foreach (invdet dr in list)
            {
                invdet h = new invdet();
                h.i_id = dr.i_id;
                h.i_bill = dr.i_bill;
                h.i_invdate = dr.i_invdate;
                h.i_item = dr.i_item;
                h.i_pri = dr.i_pri;
                h.i_qty = dr.i_qty;

                invdetList.Add(h);
            }


            detHistoryView.ItemsSource = string.Empty;
            detHistoryView.ItemsSource = invdetList;

            if (invdetList.Count() > 0)
            {
                popupDetHistoryView.IsVisible = true;
            }
            else
            {
                DisplayAlert("History", "No sold history found", "Cancel");
            }
        }


        private void btn_Invdet_Clicked(object sender, EventArgs e)
        {
            var duration = TimeSpan.FromSeconds(0.1);
            Vibration.Vibrate(duration);

            Button button = (Button)sender;
            string pri = button.CommandParameter.ToString();
            decimal npri = decimal.Parse(pri);

            E1.Text = npri.ToString("F2");

            int nQty = int.Parse(E2.Text);

            L1.Text = (npri * nQty).ToString("C2");

            //Button button = (Button)sender;
            //string pri = button.CommandParameter.ToString();
            //decimal npri = decimal.Parse(pri);

            //Grid grid_3 = (Grid)button.Parent;
            //StackLayout stack_3_1 = (StackLayout)grid_3.Parent;
            //ViewCell vc_3_1 = (ViewCell)stack_3_1.Parent;
            //ListView lv_3_1 = (ListView)vc_3_1.Parent;
            //StackLayout stack_3 = (StackLayout)lv_3_1.Parent;
            //Grid grid_1 = (Grid)stack_3.Parent;

            //StackLayout stack_2 = (StackLayout)grid_1.FindByName("slo_2");
            //Entry ent_pri = (Entry)stack_2.FindByName("txt_pri");
            //Entry ent_qty = (Entry)stack_2.FindByName("txt_qty");
            //Label lbl_damt = (Label)stack_2.FindByName("lbl_damt");

            //int nqty = int.Parse(ent_qty.Text);

            //ent_pri.Text = npri.ToString("F2");

            //lbl_damt.Text = (npri * nqty).ToString("F2");

        }



        private void txt_pri_Unfocused(object sender, FocusEventArgs e)
        {
            Entry price = (Entry)sender;

            Grid ParentGrid = (Grid)price.Parent;
            Entry txtqty = (Entry)ParentGrid.FindByName("txt_qty");
            Label lbldamt = (Label)ParentGrid.FindByName("lbl_damt");

            int ordQty = int.Parse(txtqty.Text);
            decimal ordPri = decimal.Parse(price.Text);

            lbldamt.Text = (ordPri * ordQty).ToString("C2");
        }

        private void txt_qty_Unfocused(object sender, FocusEventArgs e)
        {
            Entry entry = (Entry)sender;

            Grid ParentGrid = (Grid)entry.Parent;
            Entry txtpri = (Entry)ParentGrid.FindByName("txt_pri");
            Label lbldamt = (Label)ParentGrid.FindByName("lbl_damt");

            int ordQty = int.Parse(entry.Text);
            decimal ordPri = decimal.Parse(txtpri.Text);

            lbldamt.Text = (ordPri * ordQty).ToString("C2");
        }

        private async void RemarkPressed(object sender, FocusEventArgs e)
        {
            string _CustID = Preferences.Get("CustID", "XXXX");

            string eSwitch = (((Entry)sender).BindingContext).ToString();
            string rm = "";
            string strSql = "";

            switch (eSwitch)
            {
                case "Remark1":

                    rm = string.IsNullOrEmpty(txt_rem1.Text) ? " " : txt_rem1.Text;
                    strSql = $@"UPDATE REMARK SET r_rem1 = '{rm}' WHERE r_cust = '{_CustID}'";
                    break;
                case "Remark2":
                    rm = string.IsNullOrEmpty(txt_rem2.Text) ? " " : txt_rem2.Text;
                    string cSQL2 = $@"update REMARK set r_rem2 = '" + rm + "'" + " where r_cust = '" + _CustID + "'";
                    strSql = cSQL2;
                    break;
                case "Remark3":
                    rm = string.IsNullOrEmpty(txt_rem3.Text) ? " " : txt_rem3.Text;
                    string cSQL3 = $@"update REMARK set r_rem3 = '" + rm + "'" + " where r_cust = '" + _CustID + "'";
                    strSql = cSQL3;
                    break;
                case "Remark4":
                    rm = string.IsNullOrEmpty(txt_rem4.Text) ? " " : txt_rem4.Text;
                    string cSQL4 = $@"update REMARK set r_rem4 = '" + rm + "'" + " where r_cust = '" + _CustID + "'";
                    strSql = cSQL4;
                    break;
            }

            await LocalDb.db.ExecuteAsync(strSql);

        }

        private void btn_Close_Clicked(object sender, EventArgs e)
        {
            popupDetHistoryView.IsVisible = false;
        }
    }
}