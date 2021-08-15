using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using TaskApp.Models;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.ViewModels
{
    class MainPageViewModel : NotificationObject
    {
        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
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

        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool isNotValidForm = false;

        public bool IsNotValidForm
        {
            get { return isNotValidForm; }
            set
            {
                isNotValidForm = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogginCommand { get; set; }
        public ICommand CreateAccountCommand { get; set; }

        public MainPageViewModel()
        {
            LogginCommand = new Command(LogginCommandExecute);
            CreateAccountCommand = new Command(CreateAccountCommandExecute);
            IsEnabled = true;
        }

        private async void LogginCommandExecute()
        {
            IsLoading = true;
            IsEnabled = false;
            ErrorMessage = "";
            IsNotValidForm = false;

            if (string.IsNullOrEmpty(Email))
            {
                IsNotValidForm = true;
                ErrorMessage += "- El correo es obligatorio.";
            }

            if (string.IsNullOrEmpty(Password))
            {

                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                ErrorMessage += "- La contraseña es obligatorio.";
            }
            else if (Password.Length < 6)
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                if (!ErrorMessage.Contains("- El correo es obligatorio."))
                    ErrorMessage += "- Contraseña o correo incorrecto.";

            }

            if (IsNotValidForm)
            {
                IsLoading = false;
                IsEnabled = true;
                return;
            }


            IsNotValidForm = false;

            Login login = new Login
            {
                Email = this.Email,
                Password = this.Password
            };

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/UserApi/Login");

            var client = new HttpClient();

            StringContent json = Utils.ConvertJson(login);

            var response = await client.PostAsync(requestUri, json);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var convert = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<JsonUserId>(convert);

                var config = new ConfigUser();

                config.Key = Literals.TOKEN;
                config.Value = items.Token;
                await App.SQLiteDB.SaveConfigAsync(config);

                MessagingCenter.Send(this, Literals.GoToHomePage);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                IsNotValidForm = true;
                ErrorMessage = "";
                ErrorMessage += "Contraseña o correo incorrecto.";
            }
            else
            {
                IsNotValidForm = true;
                ErrorMessage = "";
                ErrorMessage += "No hay conexión con el servidor.";
            }

            IsLoading = false;
            IsEnabled = true;
        }

        private void CreateAccountCommandExecute()
        {
            MessagingCenter.Send(this, "GoToCreateUserPage");
        }
    }

    public class JsonUserId
    {
        public string Token { get; set; }
    }
}