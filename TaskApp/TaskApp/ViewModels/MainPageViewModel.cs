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

        public ICommand LogginCommand { get; set; }
        public ICommand CreateAccountCommand { get; set; }

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


        public MainPageViewModel()
        {
            LogginCommand = new Command(LogginCommandExecute);
            CreateAccountCommand = new Command(CreateAccountCommandExecute);

            //GetUserLogin();
        }

        private void GetUserLogin()
        {
            var properties = Application.Current.Properties;

            foreach (var item in properties)
            {
                if (item.Key == Literals.EMAIL)
                {
                    Email = item.Value.ToString();
                }

                if (item.Key == Literals.PASSWORD)
                {
                    Password = item.Value.ToString();
                }
            }

            LogginCommandExecute();
        }

        private async void LogginCommandExecute()
        {
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
            } else if (Password.Length < 6)
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                if (!ErrorMessage.Contains("- El correo es obligatorio."))
                    ErrorMessage += "- Contraseña o correo incorrecto.";

            }

            if (IsNotValidForm)
                return;


            IsNotValidForm = false;

            Login login = new Login
            {
                Email = this.Email,
                Password = this.Password
            };

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/loginapi");

            var client = new HttpClient();

            StringContent json = Utils.ConvertJson(login);

            var response = await client.PostAsync(requestUri, json);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var convert = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<JsonUserId>(convert);

                Application.Current.Properties[Literals.TOKEN] = items.Id;
                Application.Current.Properties[Literals.EMAIL] = Email;
                Application.Current.Properties[Literals.PASSWORD] = Password;

                MessagingCenter.Send(this, Literals.GoToHomePage);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                IsNotValidForm = true;
                ErrorMessage += "- Contraseña o correo incorrecto.";
            }

        }

        private void CreateAccountCommandExecute()
        {
            MessagingCenter.Send(this, "GoToCreateUserPage");
        }
    }

    public class JsonUserId
    {
        public string Id { get; set; }
    }
}