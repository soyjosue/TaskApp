using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Helper;
using TaskApp.ViewModels;
using WebApi.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<HomePageViewModel>(this, Literals.GoToLoginPage, async (a) => {
                await Navigation.PushAsync(new MainPage());
                Navigation.RemovePage(Navigation.NavigationStack.ElementAt(Navigation.NavigationStack.Count - 2));
            });

            MessagingCenter.Subscribe<HomePageViewModel>(this, Literals.GoToCreateProyectPage, async (a) =>
            {
                await Navigation.PushAsync(new CreateProyectsPage());
            });

            MessagingCenter.Subscribe<HomePageViewModel, Proyect>(this, Literals.GoToProyectPage, async (s, arg) =>
            {
                await Navigation.PushAsync(new ProyectPage(arg));
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, Literals.ReloadPage);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as HomePageViewModel;

            var proyect = e.Item as Proyect;

            vm.HideOrShowButton(proyect);
        }
    }
}
