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

            GetTaksListAsync();

            CreateTaskPageCommand = new Command(CreateTaskPageCommandExecute);
            UpdateTaskCommand = new Command(UpdateTaskCommandExecute);
            ShowOrHideTaskCompletedCommand = new Command(ShowOrHideTaskCompletedCommandExecute);

            MessagingCenter.Subscribe<CreateTaskPageViewModel>(this, Literals.ReloadPage, (sender) =>
            {
                GetTaksListAsync();
            });
        }

        private void ShowOrHideTaskCompletedCommandExecute(object obj)
        {
            var item = obj as ToolbarItem;

            if(!ShowOrHideTaskCompleted)
                item.IconImageSource = ImageSource.FromFile("IconCompleteGreen.png");
            else
                item.IconImageSource= ImageSource.FromFile("IconCompleteWhite.png");

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

        public ICommand CreateTaskPageCommand { get; }
        public ICommand UpdateTaskCommand { get; }
        public ICommand ShowOrHideTaskCompletedCommand { get; set; }

        private void CreateTaskPageCommandExecute(object obj)
        {
            MessagingCenter.Send(this, Literals.GoToCreateTaskPage);
        }

        private async void UpdateTaskCommandExecute(object obj)
        {
            if (obj != null)
            {
                var task = obj as Task;

                Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/tasks/{task.Id}");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                var json = Utils.ConvertJson(task);

                await client.PutAsync(requestUri, json);

                if(!ShowOrHideTaskCompleted)
                {
                    GetTaksListAsync();
                }
            }

        }

        private async void GetTaksListAsync()
        {
            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/tasks/{IdProyect}");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            var response = await client.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tasks = JsonConvert.DeserializeObject<ObservableCollection<Task>>(await response.Content.ReadAsStringAsync())
                    .Where(r => 
                    {
                        if(ShowOrHideTaskCompleted)
                        {
                            return true;
                        } else
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

        }
    }
}
