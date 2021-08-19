using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TaskApp.Helper;
using WebApi.Models;

namespace TaskApp.ViewModels
{
    class PeopleListPageViewModel : NotificationObject
    {
        public PeopleListPageViewModel()
        {
            users = new ObservableCollection<User> {
                new User{ Name = "Pepin", Email = "correo@correo.com", Id = 1, Lastname = "Inoa" },
                new User{ Name = "Juan", Email = "correo2@correo.com", Id = 2, Lastname = "Garcia" },
                new User{ Name = "Miguel", Email = "correo3@correo.com", Id = 3, Lastname = "Bido" },
            };
        }

        private ObservableCollection<User> users;

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

    }
}
