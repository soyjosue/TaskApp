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

            }
        }

        private async void GetCodeShared()
        {
            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/Shared/{ProyectId}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            try
            {
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var sharedProyect = JsonConvert.DeserializeObject<SharedProyect>(await response.Content.ReadAsStringAsync());
                    BarcodeValue = JsonConvert.SerializeObject(new
                    {
                        sharedCode = sharedProyect.Code,
                        passwordShared = sharedProyect.CodePassword
                    });
                    SharedCodeId = sharedProyect.Id;
                }
            }
            catch
            {

            }
        }
    }
}
