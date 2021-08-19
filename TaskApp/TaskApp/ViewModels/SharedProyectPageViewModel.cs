using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
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
            BarcodeValue = JsonConvert.SerializeObject(new
            {
                sharedCode = SharedCode,
                passwordShared = PasswordShared
            });

            PeopleListPageCommand = new Command(PeopleListPageCommandExecute);
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

        private bool isShared;

        public bool IsShared
        {
            get { return isShared; }
            set
            {
                isShared = value;
                OnPropertyChanged();
            }
        }

        private string sharedCode = Guid.NewGuid().ToString();

        public string SharedCode
        {
            get { return sharedCode; }
            set
            {
                sharedCode = value;
                OnPropertyChanged();
            }
        }

        public string passwordShared = Guid.NewGuid().ToString();

        public string PasswordShared
        {
            get { return passwordShared; }
            set
            {
                passwordShared = value;
                OnPropertyChanged();
            }
        }

        public ICommand PeopleListPageCommand { get; set; }

        private void PeopleListPageCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToPeopleListPage);
        }
    }
}
