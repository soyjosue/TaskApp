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
            Token = Utils.GetToken();

            LogOutCommand = new Command(LogOutCommandExecute);
            CreateProyectCommand = new Command(CreateProyectCommandExecute);
            ShowProyectCommand = new Command(ShowProyectCommandExecute);
            DeleteProyectCommand = new Command(DeleteProyectCommandExecute);

            MessagingCenter.Subscribe<HomePage>(this, Literals.ReloadPage, (a) =>
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

        private bool isFailedConection;

        public bool IsFailedConection
        {
            get { return isFailedConection; }
            set
            {
                isFailedConection = value;
                OnPropertyChanged();
            }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
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

            var user = await Helper.Utils.GetUser();

            if (proyect.UserId == user.Id)
                DeleteProyect(proyect);
            else
                RemoveMember(proyect, user);
        }

        private async void RemoveMember(Proyect proyect, User user)
        {
            var resultMessageConfirm = await Application.Current.MainPage.DisplayAlert("Dejar de ser miembro", $"Estas seguro de dejar de ser miembro del proyecto: {proyect.Title}?", "Si", "No");

            if (resultMessageConfirm)
            {

                Uri uriRequest = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/RemoveMember/{proyect.Id}/{user.Id}");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                try
                {
                    var response = await client.DeleteAsync(uriRequest);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        GetProyectsListAsync();
                }
                catch
                {

                }
            }

        }

        private async void DeleteProyect(Proyect proyect)
        {
            var resultMessageConfirm = await Application.Current.MainPage.DisplayAlert("Eliminar Proyecto", $"Estas seguro de eliminar el proyecto: {proyect.Title}?", "Si", "No");

            if (resultMessageConfirm)
            {
                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Delete/{proyect.Id}");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Token);

                var response = await client.DeleteAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                    GetProyectsListAsync();
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                    MessagingCenter.Send(this, Literals.GoToLoginPage);
                }
                else
                    IsFailedConection = true;
            }
        }

        private async void LogOutCommandExecute(object obj)
        {

            var resultAlert = await Application.Current.MainPage.DisplayAlert("Cerrar Sesión", "Estas seguro?", "Si", "No");

            if (resultAlert)
            {
                await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                MessagingCenter.Send(this, Literals.GoToLoginPage);
            }

        }

        private async void GetProyectsListAsync()
        {
            IsLoading = true;
            ProyectsList = null;
            IsFailedConection = false;

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Token);

            try
            {
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ProyectsList = JsonConvert.DeserializeObject<ObservableCollection<Proyect>>(await response.Content.ReadAsStringAsync());
                    if (ProyectsList.Count > 0)
                    {
                        IsFullList = true;
                        IsEmptyList = false;

                        if (ProyectsList.Count == 0)
                        {
                            IsFullList = false;
                        }
                    }
                    else
                    {
                        IsFullList = false;
                        IsEmptyList = true;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                    MessagingCenter.Send(this, Literals.GoToLoginPage);
                }

            }
            catch
            {
                IsFailedConection = true;
            }

            IsLoading = false;
        }
    }
}
