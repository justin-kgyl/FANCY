using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace FANCY
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("DataManagement", typeof(Pages.DataManagement));

        }
    }
}
