using System;
using System.IO;
using TaskApp.Helper;
using TaskApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskApp
{
    public partial class App : Application
    {
        static SQLiteHelper db;

        public App()
        {
            InitializeComponent();

            if (App.SQLiteDB.GetValueConfigUser(Literals.TOKEN) != null)
            {
                MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.FromHex("#024A86") };
            } else
            {
                MainPage = new NavigationPage(new MainPage()) { BarBackgroundColor = Color.FromHex("#024A86") };
            }
        }

        public static SQLiteHelper SQLiteDB
        {
            get
            {
                if (db == null)
                {
                    db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConfigUser.db3"));
                }

                return db;
            }
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
