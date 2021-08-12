using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using TaskApp.Views;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.ViewModels
{
    class HomePageViewModel : NotificationObject
    {
        public HomePageViewModel()
        {
            Token = Application.Current.Properties[Literals.TOKEN].ToString();

            GetProyectsListAsync();


            LogOutCommand = new Command(LogOutCommandExecute);
            CreateProyectCommand = new Command(CreateProyectCommandExecute);
            ShowProyectCommand = new Command(ShowProyectCommandExecute);
            DeleteProyectCommand = new Command(DeleteProyectCommandExecute);

            MessagingCenter.Subscribe<CreateProyectsPage>(this, Literals.ReloadPage, (a) =>
            {
                GetProyectsListAsync();
            });
        }



        private string Token { get; set; }

        private ObservableCollection<Proyect> proyectsList;

        public ObservableCollection<Proyect> ProyectsList
        {
            get { return proyectsList; }
            set
            {
                proyectsList = value;
                OnPropertyChanged();
            }
        }

        private bool isEmptyList;

        public bool IsEmptyList
        {
            get { return isEmptyList; }
            set
            {
                isEmptyList = value;
                OnPropertyChanged();
            }
        }

        private bool isFullList;

        public bool IsFullList
        {
            get { return isFullList; }
            set
            {
                isFullList = value;
                OnPropertyChanged();
            }
        }


        public ICommand LogOutCommand { get; set; }
        public ICommand CreateProyectCommand { get; set; }
        public ICommand DeleteProyectCommand { get; set; }
        public ICommand ShowProyectCommand { get; set; }

        public void HideOrShowButton(Proyect proyect)
        {
            foreach (var item in ProyectsList)
            {
                if (item.Id == proyect.Id)
                {
                    item.IsVisible = !item.IsVisible;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }

        private void ShowProyectCommandExecute(object obj)
        {
            MessagingCenter.Send<HomePageViewModel, Proyect>(this, Literals.GoToProyectPage, obj as Proyect);
        }

        private void CreateProyectCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToCreateProyectPage);
        }

        private async void DeleteProyectCommandExecute(object obj)
        {
            var proyect = obj as Proyect;

            var resultMessageConfirm = await Application.Current.MainPage.DisplayAlert("Eliminar Proyecto", $"Estas seguro de eliminar el proyecto: {proyect.Title}?", "Si", "No");

            if (resultMessageConfirm)
            {
                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/proyects/{proyect.Id}");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Token);

                var response = await client.DeleteAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    GetProyectsListAsync();
                }
            }

        }

        private async void LogOutCommandExecute(object obj)
        {

            var resultAlert = await Application.Current.MainPage.DisplayAlert("Cerrar Sesión", "Estas seguro?", "Si", "No");

            if (resultAlert)
            {
                var properties = Application.Current.Properties;

                properties.Remove(Literals.TOKEN);
                properties.Remove(Literals.EMAIL);
                properties.Remove(Literals.PASSWORD);

                MessagingCenter.Send(this, Literals.GoToLoginPage);
            }

        }

        private async void GetProyectsListAsync()
        {
            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/proyects");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Token);

            var response = await client.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ProyectsList = JsonConvert.DeserializeObject<ObservableCollection<Proyect>>(await response.Content.ReadAsStringAsync());
                if (ProyectsList.Count > 0)
                {
                    IsFullList = true;
                    IsEmptyList = false;
                }
                else
                {
                    IsFullList = false;
                    IsEmptyList = true;
                }

            }
        }
    }
}
