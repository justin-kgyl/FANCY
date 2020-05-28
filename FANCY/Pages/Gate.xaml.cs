using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using FANCY.Models;
using Newtonsoft.Json;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gate : ContentPage
    {
        public Gate()
        {
            InitializeComponent();
            if (Preferences.Get("UserID", "XXXX") == "XXXX")
            {
                txtUser.Text = "";
            }
            else
            {
                txtUser.Text = Preferences.Get("UserID", "XXXX");
            }

            txt_IP.Text = Preferences.Get("ServerIpAddress", "");

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            Shell.SetTabBarIsVisible(this, false);
        }

        private void BtnIP_Clicked(object sender, EventArgs e)
        {
            txt_IP.IsVisible = true;
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {

            string cIP = txt_IP.Text.Trim();
            
            //txtUser.Text = "SALES03";
            //txtPassword.Text = "1234";

            if (string.IsNullOrEmpty(cIP))
            {
                await DisplayAlert("IP Setup", "Please setup Server's Web(IP) address First.", "OK");
            }
            Preferences.Set("ServerIpAddress", cIP);

            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                await DisplayAlert("STOP!", "Please Type User ID and Password!", "OK");
                return;
            }
            else
            {
                if (Preferences.Get("ServerIpAddress", "XXXX") == "XXXX")
                {
                    await DisplayAlert("IP Setup","Please set up server's Web(IP) address first from the menu, setting", "OK");
                    return;
                }
                else
                {
                   
                    string userid = txtUser.Text.Trim();
                    string password = txtPassword.Text.Trim();
                    string url = Preferences.Get("ServerIpAddress", "XXXX") + "/mobile/GetUser/";
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

                        Preferences.Set("UserID", txtUser.Text.Trim());
                        Preferences.Set("UserName", mUSERs[0].Name);
                        Preferences.Set("SalesCat", mUSERs[0].Category);

                        if (mUSERs[0].Accept == "Y")
                        {
                            Application.Current.MainPage = new AppShell();
                            await Shell.Current.GoToAsync("//main");
                            //await Navigation.PushAsync(new Pages.Menu());
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

   
    }
}