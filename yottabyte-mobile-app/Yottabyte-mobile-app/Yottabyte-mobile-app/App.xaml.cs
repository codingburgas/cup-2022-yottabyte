using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Yottabyte_mobile_app.Services;
using Yottabyte_mobile_app.Views;

namespace Yottabyte_mobile_app
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
