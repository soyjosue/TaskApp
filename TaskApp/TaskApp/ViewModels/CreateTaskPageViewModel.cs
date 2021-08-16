using Newtonsoft.Json;
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
    class CreateTaskPageViewModel : NotificationObject
    {
        public CreateTaskPageViewModel(string id)
        {
            ProyectId = id;

            SaveNewTaskCommand = new Command(SaveNewTaskCommandExecute);
        }

        private async void SaveNewTaskCommandExecute(object obj)
        {
            IsLoading = true;

            if (string.IsNullOrEmpty(Titulo))
            {
                IsNotValidForm = true;
                IsLoading = false;
                MessageError = "";
                MessageError = "El nombre de la tarea es obligatorio.";
                return;
            }

            IsNotValidForm = false;
            var task = new Task
            {
                Title = Titulo
            };

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/TaskAPI/{ProyectId}/Create");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            var json = Utils.ConvertJson(new { Title = task.Title });

            try
            {
                var response = await client.PostAsync(requestUri, json);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessagingCenter.Send(this, Literals.GoToProyectPage);
                    MessagingCenter.Send(this, Literals.ReloadPage);
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

            IsLoading = false;
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


        public string ProyectId { get; set; }

        private string titulo;

        public string Titulo
        {
            get { return titulo; }
            set
            {
                titulo = value;
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


        public ICommand SaveNewTaskCommand { get; set; }
    }
}
