using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.ViewModels
{
    class PeopleListPageViewModel : NotificationObject
    {
        public PeopleListPageViewModel(Proyect proyect)
        {
            ProyectId = proyect.Id;

            DeleteMemberCommand = new Command(DeleteMemberCommandExecute);

            GetMembersOfTheProyect();
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


        private ObservableCollection<User> users;

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
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


        public ICommand DeleteMemberCommand { get; set; }

        public int ProyectId { get; set; }

        private async void DeleteMemberCommandExecute(object obj)
        {
            IsLoading = false;
            var user = obj as User;

            var resultMessageConfirm = await Application.Current.MainPage.DisplayAlert("Dejar de ser miembro", $"Estas seguro de sacar a: {user.Name} {user.Lastname}?", "Si", "No");

            if (!resultMessageConfirm)
                return;

            IsLoading = true;
            Uri uriRequest = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/RemoveMember/{ProyectId}/{user.Id}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            try
            {
                var response = await client.DeleteAsync(uriRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    GetMembersOfTheProyect();
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("No se pudo eliminar el usuario", "Error de conexión", "Ok");
            }
            IsLoading = false;
        }

        private async void GetMembersOfTheProyect()
        {
            IsLoading = false;
            IsFailedConection = false;
            users = null;
            IsEmptyList = false;

            Uri uriRequest = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Members/{ProyectId}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            try
            {
                var response = await client.GetAsync(uriRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(await response.Content.ReadAsStringAsync());

                    if (Users.Count == 0)
                        IsEmptyList = true;
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
