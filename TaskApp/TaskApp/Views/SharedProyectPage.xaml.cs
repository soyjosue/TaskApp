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
    public partial class SharedProyectPage : ContentPage
    {
        public SharedProyectPage(Proyect proyect)
        {
            InitializeComponent();

            BindingContext = new SharedProyectPageViewModel(proyect);

            MessagingCenter.Subscribe<SharedProyectPageViewModel>(this, Literals.GoToPeopleListPage, async (s) =>
            {
                await Navigation.PushAsync(new PeopleListPage(proyect));
            });
        }
    }
}