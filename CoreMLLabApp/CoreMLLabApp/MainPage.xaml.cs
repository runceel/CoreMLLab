using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using Xamarin.Forms;

namespace CoreMLLabApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
            if (file == null) { return; }

            this.image.Source = ImageSource.FromStream(() => file.GetStream());

            using (var fs = file.GetStream())
            using (var ms = new MemoryStream())
            {
                await fs.CopyToAsync(ms);
                var d = DependencyService.Get<IFriesOrNotFriesService>();
                var result = await d.DetectAsync(ms.ToArray());
                await this.DisplayAlert("Result", result, "OK");
            }
        }
    }
}
