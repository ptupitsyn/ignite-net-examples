using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IgniteMobileXamarinForms.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://ignite.apache.org"));
        }

        public ICommand OpenWebCommand { get; }
    }
}