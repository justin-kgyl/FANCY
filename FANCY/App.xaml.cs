using FANCY.Pages;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY
{   
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjQwMzA5QDMxMzgyZTMxMmUzMFFLY3JzVFJ2b2F2ZjhLM0phdEF2Z3JERDBudVVNRHhYbUNteDEwT1BnMms9");
            InitializeComponent();

            MainPage = new AppShell();

            if (Preferences.Get("UserID", "XXXX") == "XXXX")
            {
                MainPage = new Gate();
            }
            else
            {
                MainPage = new AppShell();
            }

            //test1111 hi t=her
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
