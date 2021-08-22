using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using TaskAppBackend.Models;
using WebApi.Models;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace TaskApp.ViewModels
{
    class SharedProyectPageViewModel : NotificationObject
    {

        public SharedProyectPageViewModel(Proyect proyect)
        {
            ProyectTitle = proyect.Title;
            ProyectId = proyect.Id;

            PeopleListPageCommand = new Command(PeopleListPageCommandExecute);
            ChangeCodeCommand = new Command(ChangeCodeCommandExecute);

            GetCodeShared();
        }

        private string barcodeValue;

        public string BarcodeValue
        {
            get { return barcodeValue; }
            set
            {
                barcodeValue = value;
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



        public string ProyectTitle { get; set; }

        public ICommand PeopleListPageCommand { get; set; }
        public ICommand ChangeCodeCommand { get; set; }

        public int SharedCodeId { get; set; }
        public int ProyectId { get; set; }

        private void PeopleListPageCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToPeopleListPage);
        }

        private async void ChangeCodeCommandExecute(object obj)
        {
            var isCorrect = await Application.Current.MainPage.DisplayAlert("Cambiar codigo QR", "Estas seguro de cambiar el codigo QR?", "Si", "No");

            if (!isCorrect)
                return;

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Shared/{SharedCodeId}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            var json = Utils.ConvertJson(new { });

            try
            {
                var response = await client.PutAsync(requestUri, json);

                if (response.StatusCode == HttpStatusCode.OK)
                    GetCodeShared();
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("No se pudo cambiar el QR", "Verifique su conexión a internet.", "OK");
            }
        }

        private async void GetCodeShared()
        {
            IsFailedConection = false;
            IsLoading = true;
            BarcodeValue = null;

            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Shared/{ProyectId}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            try
            {
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var sharedProyect = JsonConvert.DeserializeObject<SharedProyect>(await response.Content.ReadAsStringAsync());
                    BarcodeValue = JsonConvert.SerializeObject(new SharedProyect
                    {
                        Code = sharedProyect.Code,
                        CodePassword = sharedProyect.CodePassword
                    });
                    SharedCodeId = sharedProyect.Id;
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
