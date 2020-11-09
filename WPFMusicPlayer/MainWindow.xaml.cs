using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFMusicPlayer.Model;

namespace WPFMusicPlayer
{
    public partial class MainWindow : Window, IMPView
    {
        private readonly Presenter _presenter;


        public MainWindow()
        {
            InitializeComponent();

            _presenter = new Presenter(this);
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            _presenter.OpenFile();
        }

        private void ListBoxTrackList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedItem == null)
                return;
            _presenter.SelectTrack(((ListBox) sender).SelectedIndex);
        }

        private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
        {
            _presenter.PlayOrPause();
        }

        private void SliderProgress_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _presenter.ProgressChange(((Slider) sender).Value);
        }

        private void SliderVolume_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _presenter?.VolumeChange((int)((Slider) sender).Value);
        }

        private void ButtonNextTrack_Click(object sender, RoutedEventArgs e)
        {
            _presenter.NextTrack();
        }

        private void ButtonPreviousTrack_Click(object sender, RoutedEventArgs e)
        {
            _presenter.PreviousTrack();
        }

        private void ButtonMoveProgressFurther_Click(object sender, RoutedEventArgs e)
        {
            _presenter.MoveProgressFurther();
        }

        private void ButtonMoveProgressBack_Click(object sender, RoutedEventArgs e)
        {
            _presenter.MoveProgressBack();
        }

        private void ButtonIncreaseVolume_Click(object sender, RoutedEventArgs e)
        {
            _presenter.IncreaseVolume();
        }

        private void ButtonDecreaseVolume_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DecreaseVolume();
        }

        private void ButtonNextTrackSetting_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ChangeNextTrackSetting();
        }

        private void ButtonClearTrackList_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ClearAll();
        }

        public void AddToTrackList(string[] trackList)
        {
            foreach (var track in trackList)
            {
                ListBoxTrackList.Items.Add(track);
            }
        }

        public void SetTrackTitle(string title)
        {
            TextBlockTrackTitle.Text = title;
        }

        public void SetTrackProgress(TimeSpan progress)
        {
            TextBlockTrackProgress.Text = progress.ToString("mm\\:ss");
        }

        public void SetTrackDuration(TimeSpan duration)
        {
            TextBlockTrackDuration.Text = duration.ToString("mm\\:ss");
        }

        public void SetVolume(int volume)
        {
            SliderVolume.Value = volume;
        }

        public void SetProgress(int progress)
        {
            SliderProgress.Value = progress;
        }

        public void SetMaximumProgress(double maximumProgress)
        {
            SliderProgress.Maximum = maximumProgress;
        }

        public void SetPlay()
        {
            ButtonPlayPause.Content = new Image()
                {Source = new BitmapImage(new Uri("Resources/image_pause.png", UriKind.Relative))};
        }

        public void SetPause()
        {
            ButtonPlayPause.Content = new Image()
                { Source = new BitmapImage(new Uri("Resources/image_play.png", UriKind.Relative)) };
        }

        public void SetSelectedTrack(string title)
        {
            ListBoxTrackList.SelectedItem = title;
        }

        public void SetNextTrackSetting(NextTrackSetting setting)
        {
            var image = new Image();
            switch (setting)
            {
                case NextTrackSetting.AutoNext:
                    image.Source = new BitmapImage(new Uri("Resources/image_autonext.png", UriKind.Relative));
                    break;
                case NextTrackSetting.Shuffle:
                    image.Source = new BitmapImage(new Uri("Resources/image_shuffle.png", UriKind.Relative));
                    break;
                case NextTrackSetting.AutoStop:
                    image.Source = new BitmapImage(new Uri("Resources/image_autostop.png", UriKind.Relative));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
            }
            ButtonNextTrackSetting.Content = image;
        }

        public void ClearAll()
        {
            ListBoxTrackList.Items.Clear();
        }

        private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Would you like to see my other projects on GitHub?",
                "Hey!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start("https://github.com/VengerAndrey");
            }
        }
    }
}
