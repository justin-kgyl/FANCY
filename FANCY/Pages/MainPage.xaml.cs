using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using FANCY.Models;
using Newtonsoft.Json;

namespace FANCY.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private List<USER> mUSERs;
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            Preferences.Set("ServerIpAddress", "184.75.36.75:8084");
            //Loading();
        }

        public void Loading()
        {
            if (Preferences.Get("ServerIpAddress", "XXXX") == "XXXX")
            {
                txtServerIpAddress.Text = "";
            }
            else
            {
                txtServerIpAddress.Text = Preferences.Get("ServerIpAddress", "XXXX");
            }

        }
        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {

            txtUser.Text = "SALES03";
            txtPassword.Text = "1234";
            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                await DisplayAlert("STOP!", "Please Type User ID and Password!", "OK");
                return;
            }
            else
            {
                if (Preferences.Get("ServerIpAddress", "XXXX") == "XXXX")
                {
                    await DisplayAlert("STOP !", "Enter Server IP Address First", "OK");
                    return;
                }
                else
                {
                    Preferences.Set("UserID", txtUser.Text.Trim());
                    string userid = txtUser.Text.Trim();
                    string password = txtPassword.Text.Trim();
                    string url = "http://" + Preferences.Get("ServerIpAddress", "XXXX") + "/mobile/GetUser/";
                    var client = new HttpClient();
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("userid", userid),
                        new KeyValuePair<string, string>("password", password)
                    });


                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception("API is not configured properly!");
                        }

                        string responseBody = await response.Content.ReadAsStringAsync();
                        var mUSERs = JsonConvert.DeserializeObject<List<USER>>(responseBody);

                        Preferences.Set("SalesRep", mUSERs[0].Sale);
                        Preferences.Set("SalesCat", mUSERs[0].Category);

                        if (mUSERs[0].Accept == "Y")
                        {
                            await Navigation.PushAsync(new Pages.Menu());
                        }
                        else
                        {
                            await DisplayAlert("Please Try Again", "No Such User ID and Password Existed", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
        }

        private void MenuItem1_Clicked(object sender, EventArgs e)
        {
            popupSetupView.IsVisible = true;

            if (Preferences.Get("ServerIpAddress", "XXXX") == "XXXX")
            {
                txtServerIpAddress.Text = "";
            }
            else
            {
                txtServerIpAddress.Text = Preferences.Get("ServerIpAddress", "XXXX");
            }

        }
        private void BtnSetupEnter_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("ServerIpAddress", txtServerIpAddress.Text.Trim());
            popupSetupView.IsVisible = false;
        }
    }
}
