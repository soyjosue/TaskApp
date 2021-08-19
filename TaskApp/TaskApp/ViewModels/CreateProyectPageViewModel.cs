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

        private string messageError;

        public string MessageError
        {
            get { return messageError; }
            set
            {
                messageError = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveProyectCommand { get; set; }
        public ICommand CodigoQRCommand { get; set; }

        public CreateProyectPageViewModel()
        {
            SaveProyectCommand = new Command(SaveProyectCommandExecute);
            CodigoQRCommand = new Command(CodigoQRCommandExecute);
        }

        private void CodigoQRCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToQRScannerPage);
        }

        private async void SaveProyectCommandExecute(object obj)
        {
            IsLoading = true;

            if (!string.IsNullOrEmpty(Title))
            {
                IsNotValidForm = false;
                var proyect = new Proyect
                {
                    Title = this.Title,
                };

                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Create/");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                var json = Utils.ConvertJson(proyect);

                try
                {
                    var response = await client.PostAsync(requestUri, json);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        MessagingCenter.Send(this, Literals.GoToHomePage);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                        MessagingCenter.Send(this, Literals.GoToLoginPage);
                    }
                }
                catch
                {
                    IsNotValidForm = true;
                    MessageError = "No hay conexión con el servidor.";
                }
            }
            else
            {
                IsNotValidForm = true;
                MessageError = "El nombre del proyecto es obligatorio.";
            }

            IsLoading = false;
        }
    }
}
