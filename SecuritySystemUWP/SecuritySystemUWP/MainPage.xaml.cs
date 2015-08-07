﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SecuritySystemUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static IStorage storage;
        private static ICamera camera;
        private string[] cameras = { "Cam1" };
        private static DispatcherTimer uploadPicturesTimer;
        private static DispatcherTimer deletePicturesTimer;

        private static bool started = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task Initialize()
        {
            camera = CameraFactory.Get(Config.CameraType);
            storage = StorageFactory.Get(Config.StorageProvider);

            await camera.Initialize();

            //Timer controlling camera pictures with motion
            uploadPicturesTimer = new DispatcherTimer();
            uploadPicturesTimer.Interval = TimeSpan.FromSeconds(10);
            uploadPicturesTimer.Tick += uploadPicturesTimer_Tick;
            uploadPicturesTimer.Start();

            //Timer controlling deletion of old pictures
            deletePicturesTimer = new DispatcherTimer();
            deletePicturesTimer.Interval = TimeSpan.FromHours(1);
            deletePicturesTimer.Tick += deletePicturesTimer_Tick;
            deletePicturesTimer.Start();
        }

        private void Dispose()
        {
            camera.Dispose();
            uploadPicturesTimer.Stop();
            deletePicturesTimer.Stop();
        }

        private async void RunningToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!started)
            {
                await Initialize();
                started = true;
                this.Frame.Navigate(storage.StorageStartPage());
            }
            else
            {
                Dispose();
                started = false;
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RunningToggle.Content = started ? "Stop" : "Start";
        }

        private void uploadPicturesTimer_Tick(object sender, object e)
        {
            storage.UploadPictures(cameras[0]);
        }

        private void deletePicturesTimer_Tick(object sender, object e)
        {
            storage.DeleteExpiredPictures(cameras[0]);
        }
    }
}
