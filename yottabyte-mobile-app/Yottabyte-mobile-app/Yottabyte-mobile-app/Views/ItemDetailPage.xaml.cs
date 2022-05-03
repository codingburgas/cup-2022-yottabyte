using System.ComponentModel;
using Xamarin.Forms;
using Yottabyte_mobile_app.ViewModels;

namespace Yottabyte_mobile_app.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}