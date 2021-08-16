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
    class CreateUserViewModel : NotificationObject
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string lastname;

        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; OnPropertyChanged(); }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        private string repetir;

        public string Repetir
        {
            get { return repetir; }
            set { repetir = value; OnPropertyChanged(); }
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

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateUserCommand { get; set; }

        public CreateUserViewModel()
        {
            CreateUserCommand = new Command(CreateUserCommandExecute);
        }

        private async void CreateUserCommandExecute(object obj)
        {
            ErrorMessage = "";
            IsNotValidForm = false;
            IsLoading = true;
            IsEnabled = false;

            if (string.IsNullOrEmpty(Name))
            {
                IsNotValidForm = true;
                ErrorMessage += "- El nombre es obligatorio";
            }
            if (string.IsNullOrEmpty(Lastname))
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                ErrorMessage += "- El apellido es obligatorio";
            }
            if (string.IsNullOrEmpty(Email))
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;
                ErrorMessage += "- El correo es obligatorio";
            }
            if (string.IsNullOrEmpty(Password))
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                ErrorMessage += "- La contraseña es obligatorio";
            }
            else if (Password.Length < 6)
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                ErrorMessage += "- La contraseña debe ser mayor a 6 caracteres";
            }
            else if (Password != Repetir)
            {
                if (IsNotValidForm)
                    ErrorMessage += "\n";
                else
                    IsNotValidForm = true;

                ErrorMessage += "- Las contraseñas no coinciden.";
            }

            if (IsNotValidForm)
            {
                IsLoading = false;
                IsEnabled = true;
                return;
            }

            IsNotValidForm = false;

            User user = new User()
            {
                Name = this.Name,
                Lastname = this.Lastname,
                Email = this.Email,
                Password = this.Password
            };

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/UserApi/Create");

            var client = new HttpClient();

            StringContent convertJson = Utils.ConvertJson(user);

            try
            {
                var response = await client.PostAsync(requestUri, convertJson);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessagingCenter.Send(this, Literals.GoToLoginPage);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    IsNotValidForm = true;
                    ErrorMessage = "";
                    ErrorMessage += "- El correo es utilizado por otro usuario.";
                }
            }
            catch
            {
                IsNotValidForm = true;
                ErrorMessage = "No hay conexión con el servidor.";
            }
                IsLoading = false;
                IsEnabled = true;
        }
    }
}
