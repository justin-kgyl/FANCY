using FANCY.Models;
using FANCY.ProjectClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FANCY.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Item : ContentPage
    {
        KGYLDatabase LocalDb = new KGYLDatabase();
        public Item()
       {
            InitializeComponent();
            Title = "All Items in My Device";
            Show_ItemList();
        }

       

        public void Show_ItemList()
        {
            string sql = "";

            
            
                sql = "";
                sql += "SELECT * ";
                sql += "FROM IT ";
            
           
            
            var list = LocalDb.db.QueryAsync<IT>(sql).Result.ToList();

           
                ItemListView.ItemsSource = list;
            

        }

    }
}        