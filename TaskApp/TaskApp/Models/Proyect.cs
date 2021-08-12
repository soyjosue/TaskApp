using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskApp.Helper;

namespace WebApi.Models
{
    public class Proyect : NotificationObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }


        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }

    }
}