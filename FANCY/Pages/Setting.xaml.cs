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
    public partial class Setting : ContentPage
    {
        public Setting()
        {
            InitializeComponent();
            Title = "Settings";

            if (Preferences.Get("ServerIpAddress", "XXXX")=="XXXX")
            {
                txt_IPAddress.Text = "";
            }
            else
            {
                txt_IPAddress.Text = Preferences.Get("ServerIpAddress", "XXXX");
            }
        }

        private async void Btn_Save_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_IPAddress.Text.Trim()))
            {
                Preferences.Set("ServerIpAddress", "XXXX");
            }
            else
            {
                Preferences.Set("ServerIpAddress", txt_IPAddress.Text.Trim());
            }
            
            await DisplayAlert("Web Address","Web Address saved", "OK");

            if (Preferences.Get("UserID", "XXXX") == "XXXX")
            {
                App.Current.MainPage = new Gate();
            }
            else
            {
                App.Current.MainPage = new AppShell();
            }
        }
        private async void Btn_Clear_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("UserID", "XXXX");
            Preferences.Set("UserName", "XXXX");
            Preferences.Set("SalesCat", "XXXX");
            
            Preferences.Set("CustName", "XXXX");
            Preferences.Set("FullName", "NO");
            Preferences.Set("FullAddress", "NONE");
            Preferences.Set("FromOrderDetail", "NO");

 
            await DisplayAlert("Clear Cache", "Successfully Cleaned","OK");

            Navigation.NavigationStack.ToList().Clear();

            Shell.SetTabBarIsVisible(this, false);
            
           App.Current.MainPage = new Gate();
   
        }

    }
    }