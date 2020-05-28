using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemG: ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();

        public ItemG()
        {
            InitializeComponent();
            Title = "Item Group List";
            Show_ItemGlist("All");
        }

       

        public async void Show_ItemGlist(string par1)
        {
            string sql = "";

            string search = "";
            search = txt_ITG.Text.ToUpper().Trim();

            if (par1 == "All")
            {
                sql = "";
                sql += "SELECT * ";
                sql += "FROM ITG ";
            }
            else
            {

                if (string.IsNullOrWhiteSpace(txt_ITG.Text) == false)
                {

                    sql = "";
                    sql += "SELECT * ";
                    sql += "FROM ITG ";
                    sql += "WHERE I_ID LIKE '%" + search + "%'";
                }
                else
                {
                    await DisplayAlert("Alert!", "Enter Item Group", "OK");
                    return;
                }

            }
            var list = LocalDb.db.QueryAsync<ITG>(sql).Result.ToList();

            if (list.Count() == 0)
            {
                await DisplayAlert("Alert!", "No Such Item Group Exist!", "OK");
                ItemGListView.ItemsSource = string.Empty;
                return;
            }
            else
            {
                ItemGListView.ItemsSource = list;
            }

        }

        private void Btn_All_Clicked(object sender, EventArgs e)
        {
            Show_ItemGlist("");
        }

        private void ShowItemG_Clicked(object sender, EventArgs e)
        {
            Show_ItemGlist("");
        }
    }
}