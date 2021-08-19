using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Helper;
using TaskApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateProyectsPage : ContentPage
    {
        public CreateProyectsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<CreateProyectPageViewModel>(this, Literals.GoToHomePage, async (a) =>
            {
                MessagingCenter.Send(this, Literals.ReloadPage);
                await Navigation.PopAsync();
            });

            MessagingCenter.Subscribe<CreateProyectPageViewModel>(this, Literals.GoToQRScannerPage, async (a) =>
            {
                await Navigation.PushAsync(new QRScannerPage());
            });

        }
    }
}