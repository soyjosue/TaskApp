using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TaskApp.Custom
{
    public class CustomViewCell : ViewCell
    {
        public static readonly BindableProperty SelectedItemBackgroundColorProperty =
            BindableProperty.Create("SelectedItemBackgroundColor", typeof(Color), typeof(CustomViewCell), Color.White);
       
        public Color SelectedItemBackgroundColor
        {
            get { return (Color) GetValue(SelectedItemBackgroundColorProperty); }
            set { SetValue(SelectedItemBackgroundColorProperty, value); }
        }

    }
}
