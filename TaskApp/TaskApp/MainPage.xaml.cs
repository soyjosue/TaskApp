using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Helper;
using TaskApp.ViewModels;
using TaskApp.Views;
using Xamarin.Forms;

namespace TaskApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainPageViewModel>(this, Literals.GoToHomePage, async (a) =>
            {
                await Navigation.PushAsync(new HomePage());
                Navigation.RemovePage(Navigation.NavigationStack.ElementAt(Navigation.NavigationStack.Count - 2));
            });

            MessagingCenter.Subscribe<MainPageViewModel>(this, "GoToCreateUserPage", async (a) =>
            {
                await Navigation.PushAsync(new CreateUserPage());
            });

        }
    }
}
