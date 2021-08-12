using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TaskApp.Custom;
using TaskApp.Droid.Custom;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace TaskApp.Droid.Custom
{
    public class CustomViewCellRenderer: ViewCellRenderer
    {
        private Android.Views.View _cellCore;
        private Drawable _unSelectedBackground;
        private bool _selected;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _cellCore = base.GetCellCore(item, convertView, parent, context);
            _selected = false;
            _unSelectedBackground = _cellCore.Background;
            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);
            if(e.PropertyName == "IsSelected")
            {
                _selected = !_selected;
                if(_selected)
                {
                    var extendedViewCell = sender as CustomViewCell;

                    _cellCore.SetBackgroundColor(extendedViewCell.SelectedItemBackgroundColor.ToAndroid());
                } else
                {
                    _cellCore.SetBackground(_unSelectedBackground);
                }
            }
        }
    }
}