using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.ViewModels
{
    class CreateProyectPageViewModel : NotificationObject
    {
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private bool isNotValidForm;

        public bool IsNotValidForm
        {
            get { return isNotValidForm; }
            set
            {
                isNotValidForm = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveProyectCommand { get; set; }

        public CreateProyectPageViewModel()
        {
            SaveProyectCommand = new Command(SaveProyectCommandExecute);
        }

        private async void SaveProyectCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                IsNotValidForm = false;
                var proyect = new Proyect
                {
                    Title = this.Title,
                };

                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/proyects/");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                var json = Utils.ConvertJson(proyect);

                var response = await client.PostAsync(requestUri, json);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessagingCenter.Send(this, Literals.GoToHomePage);
                }
            }
            else
            {
                IsNotValidForm = true;
            }
        }
    }
}
