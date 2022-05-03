using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Yottabyte_mobile_app.ViewModels;
using Yottabyte_mobile_app.Views;

namespace Yottabyte_mobile_app
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
