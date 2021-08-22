using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TaskApp.Helper;
using TaskApp.Views;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.ViewModels
{
    class ProyectPageViewModel : NotificationObject
    {
        public ProyectPageViewModel(Proyect proyect)
        {
            ProyectName = proyect.Title;
            IdProyect = proyect.Id.ToString();

            IsTheUserOwner(proyect);

            GetTaksListAsync();

            CreateTaskPageCommand = new Command(CreateTaskPageCommandExecute);
            UpdateTaskCommand = new Command(UpdateTaskCommandExecute);
            ShowOrHideTaskCompletedCommand = new Command(ShowOrHideTaskCompletedCommandExecute);
            SharedProyectCommand = new Command(SharedProyectCommandExecute);

            MessagingCenter.Subscribe<CreateTaskPageViewModel>(this, Literals.ReloadPage, (sender) =>
            {
                GetTaksListAsync();
            });
        }

        private async void IsTheUserOwner(Proyect proyect)
        {
            var userToken = await Utils.GetUser();

            if (proyect.UserId == userToken.Id)
                IsEnabledItem = true;
            else
                IsEnabledItem = false;
        }

        private void ShowOrHideTaskCompletedCommandExecute(object obj)
        {
            var item = obj as ToolbarItem;

            if (!ShowOrHideTaskCompleted)
                item.IconImageSource = ImageSource.FromFile("IconCompleteGreen.png");
            else
                item.IconImageSource = ImageSource.FromFile("IconCompleteWhite.png");

            ShowOrHideTaskCompleted = !ShowOrHideTaskCompleted;

            GetTaksListAsync();
        }

        private bool showOrHideTaskCompleted;

        public bool ShowOrHideTaskCompleted
        {
            get { return showOrHideTaskCompleted; }
            set
            {
                showOrHideTaskCompleted = value;
                OnPropertyChanged();
            }
        }


        public string IdProyect { get; set; }
        public string ProyectName { get; set; }

        private bool isEnabledItem = true;

        public bool IsEnabledItem
        {
            get { return isEnabledItem; }
            set
            {
                isEnabledItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Task> tasksList;

        public ObservableCollection<Task> TasksList
        {
            get { return tasksList; }
            set
            {
                tasksList = value;
                OnPropertyChanged();
            }
        }

        private bool isEmptyList;

        public bool IsEmptyList
        {
            get { return isEmptyList; }
            set
            {
                isEmptyList = value;
                OnPropertyChanged();
            }
        }

        private bool isFullList;

        public bool IsFullList
        {
            get { return isFullList; }
            set
            {
                isFullList = value;
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


        public ICommand CreateTaskPageCommand { get; }
        public ICommand UpdateTaskCommand { get; }
        public ICommand ShowOrHideTaskCompletedCommand { get; set; }
        public ICommand SharedProyectCommand { get; set; }

        private void CreateTaskPageCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToCreateTaskPage);
        }

        private async void UpdateTaskCommandExecute(object obj)
        {
            if (obj != null)
            {
                var task = obj as Task;

                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/TaskApi/Edit/{task.Id}");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                var json = Utils.ConvertJson(task);

                var response = await client.PutAsync(requestUri, json);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (!ShowOrHideTaskCompleted)
                    {
                        GetTaksListAsync();
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                    MessagingCenter.Send(this, Literals.GoToLoginPage);
                }
            }

        }

        private async void GetTaksListAsync()
        {
            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/TaskApi/{IdProyect}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            var response = await client.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tasks = JsonConvert.DeserializeObject<ObservableCollection<Task>>(await response.Content.ReadAsStringAsync())
                    .Where(r =>
                    {
                        if (ShowOrHideTaskCompleted)
                        {
                            return true;
                        }
                        else
                        {
                            return !r.IsChecked;
                        }
                    });
                TasksList = new ObservableCollection<Task>(tasks);
                if (TasksList.Count > 0)
                {
                    IsFullList = true;
                    IsEmptyList = false;
                }
                else
                {
                    IsFullList = false;
                    IsEmptyList = true;
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await App.SQLiteDB.RemoveConfigUserAsync(Literals.TOKEN);

                MessagingCenter.Send(this, Literals.GoToLoginPage);
            }

        }

        private void SharedProyectCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToSharedProyectPage);
        }
    }
}
