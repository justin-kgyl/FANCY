using FANCY.Models;
using FANCY.ProjectClass;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerInfo : ContentPage
    {
        string _CustID = "";
        KGYLDatabase LocalDb = new KGYLDatabase();

        public CustomerInfo(string par1)
        {
            InitializeComponent();
            _CustID = par1.Trim();

            Title = _CustID;
            Cust_Info();
            Show_ARAging();
        }

        public void Cust_Info()
        {
            string sql;

            sql = "";
            sql += "SELECT * ";
            sql += "FROM CUST ";
            sql += "WHERE C_ID = '"  + _CustID + "'";
  
            var list = LocalDb.db.QueryAsync<CUST>(sql).Result.ToList();

            lbl_FullAddress.Text = list[0].FullAddress.ToString();
            lbl_NAME.Text = list[0].C_NAME.ToString();
            //lbl_REGION.Text = list[0].C_REGION.ToString();
            lbl_TERM.Text = list[0].C_TERM.ToString();
            //lbl_SALESREP.Text = list[0].C_SALESREP.ToString();
            lbl_BALANCE.Text = list[0].C_BALANCE.ToString();
            lbl_TEL.Text = list[0].C_TEL.ToString();
            lbl_EMAIL.Text = list[0].C_EMAIL.ToString();
            lbl_ATTN.Text = list[0].C_ATTN.ToString();

        }

        public async void Show_ARAging()
        {
            string sql = "";

            if(string.IsNullOrWhiteSpace(_CustID))
            {
                await DisplayAlert("Alert!", "Enter Customer Name", "OK");
                return;
            }
            else
            {
                sql = "";
                sql += "SELECT * ";
                sql += "FROM INV ";
                sql += "WHERE I_BILL ='" + _CustID + "' AND I_INVBAL != 0.00 ";
            

                var list = LocalDb.db.QueryAsync<inv>(sql).Result.ToList();

                if (list.Count() > 0)
                {
                    //CustARView.ItemsSource = string.Empty;
                    CustARView.ItemsSource = list;
                    return;
                }
            }
            return;
        }

        private async void Btn_Invlist_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new Invoice(_CustID));

            //Show_Custlist("All");
        }

        private async void btn_quick_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Cart_Add();
            Preferences.Set("PageFrom", "Customer");
            await Navigation.PushAsync(page);
        }

        private async void btn_multi_Clicked(object sender, EventArgs e)
        {
            Page page = new Pages.Cart_New();
            Preferences.Set("PageFrom", "Customer");
            await Navigation.PushAsync(page);
        }

        private void lbl_TEL_Tapped(object sender, EventArgs e)
        {
            try
            {
                PhoneDialer.Open(lbl_TEL.Text);
            }
            catch (Exception ex)
            {
                DisplayAlert("Unable to make Call", ex.Message, "Ok");
            }
        }

        private async void lbl_EMAIL_Tapped(object sender, EventArgs e)
        {
            List<string> recipients = new List<string>();

            string cEmail = lbl_EMAIL.Text.Trim();

            if (string.IsNullOrEmpty(cEmail))
            {
                await DisplayAlert("Email", "Email Address does not Exist.", "Confirm");
                return;
            }

            recipients.Add(cEmail);

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Email Message",
                    Body = "",
                    To = recipients
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }

        }
    }
}