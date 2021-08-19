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
    public partial class ProyectPage : ContentPage
    {
        public ProyectPage(Proyect proyect)
        {
            InitializeComponent();

            BindingContext = new ProyectPageViewModel(proyect);

            MessagingCenter.Subscribe<ProyectPageViewModel>(this, Literals.GoToCreateTaskPage, async (sender) =>
            {
                await Navigation.PushAsync(new CreateTaskPage(proyect.Id.ToString()));
            });

            MessagingCenter.Subscribe<ProyectPageViewModel>(this, Literals.GoToSharedProyectPage, async (sender) =>
            {
                await Navigation.PushAsync(new SharedProyectPage(proyect));
            });
        }
    }
}