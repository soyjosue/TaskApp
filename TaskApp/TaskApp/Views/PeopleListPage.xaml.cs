﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeopleListPage : ContentPage
    {
        public PeopleListPage()
        {
            InitializeComponent();

            BindingContext = new PeopleListPageViewModel();
        }
    }
}