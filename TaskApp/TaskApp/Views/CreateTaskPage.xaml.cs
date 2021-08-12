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
    public partial class CreateTaskPage : ContentPage
    {
        public CreateTaskPage(string id)
        {
            InitializeComponent();

            BindingContext = new CreateTaskPageViewModel(id);

            MessagingCenter.Subscribe<CreateTaskPageViewModel>(this, Literals.GoToProyectPage, async (sender) =>
            {
                await Navigation.PopAsync();
            });
        }
    }
}