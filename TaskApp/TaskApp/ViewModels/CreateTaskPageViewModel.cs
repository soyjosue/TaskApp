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
            if (string.IsNullOrEmpty(Titulo))
            {
                IsNotValidForm = true;
                return;
            }

            IsNotValidForm = false;
            var task = new Task
            {
                Title = Titulo,
                IsChecked = false
            };

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/tasks/{ProyectId}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            var json = Utils.ConvertJson(task);

            var response = await client.PostAsync(requestUri, json);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                MessagingCenter.Send(this, Literals.GoToProyectPage);
                MessagingCenter.Send(this, Literals.ReloadPage);
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
