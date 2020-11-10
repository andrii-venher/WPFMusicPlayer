using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using WPFMusicPlayer.Model;

namespace WPFMusicPlayer
{
    class Presenter
    {
        internal class PresenterSettings
        {
            public int ProgressStep { get; set; } = 5;
            public int VolumeStep { get; set; } = 10;
        }

        private readonly MusicPlayer _player;
        private readonly IMPView _view;
        private readonly PresenterSettings _settings;

        public Presenter(IMPView view, PresenterSettings settings = null)
        {
            _player = new MusicPlayer();
            _view = view;

            _settings = settings ?? new PresenterSettings();

            _view.SetNextTrackSetting(_player.NextTrackSelection);
            _view.SetVolume(_player.Volume);

            _player.PlayerStatusChanged += (o, status) =>
            {
                if (status == Status.Playing)
                {
                    _view.SetPlay();
                }
                else
                {
                    _view.SetPause();
                }
            };

            _player.TrackSelected += (o, track) =>
            {
                _view.SetMaximumProgress(track.DurationRaw);
                _view.SetTrackTitle(track.Title);
                _view.SetTrackProgress(TimeSpan.Zero);
                _view.SetTrackDuration(track.Duration);
                _view.SetSelectedTrack(track.Title);
            };

            _player.ProgressChanged += (o, progressRaw) =>
            {
                _view.SetProgress((int)progressRaw);
                _view.SetTrackProgress(((MusicPlayer) o).Progress);
            };

            _player.VolumeChanged += (o, volume) =>
            {
                _view.SetVolume(volume);
            };
        }

        public void OpenFile()
        {
            var dialog = new OpenFileDialog {Multiselect = true, Filter = "Audio files|*.mp3" };
            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    _player.LoadTrack(fileName);
                }

                _view.AddToTrackList(dialog.FileNames.Select(Path.GetFileNameWithoutExtension).ToArray());
            }
        }

        public void PlayOrPause()
        {
            if (_player.PlayerStatus == Status.Playing)
            {
                _player.Pause();
                _view.SetPause();
            }
            else
            {
                _player.Play();
                if (_player.PlayerStatus != Status.Playing)
                    _player.SelectTrack(0);
                if(_player.PlayerStatus == Status.Playing)
                    _view.SetPlay();
            }
        }

        public void ProgressChange(double progress)
        {
            _player.ProgressRaw = progress;
        }

        public void VolumeChange(int volume)
        {
            _player.Volume = volume;
        }

        public void NextTrack()
        {
            _player.SelectNextTrack();
        }

        public void PreviousTrack()
        {
            _player.SelectPreviousTrack();
        }

        public void MoveProgressFurther()
        {
            _player.ProgressRaw += _settings.ProgressStep;
        }

        public void MoveProgressBack()
        {
            _player.ProgressRaw -= _settings.ProgressStep;
        }

        public void IncreaseVolume()
        {
            _player.Volume += _settings.VolumeStep;
        }

        public void DecreaseVolume()
        {
            _player.Volume -= _settings.VolumeStep;
        }

        public void ChangeNextTrackSetting()
        {
            switch (_player.NextTrackSelection)
            {
                case NextTrackSetting.AutoNext:
                    _player.NextTrackSelection = NextTrackSetting.Shuffle;
                    _view.SetNextTrackSetting(NextTrackSetting.Shuffle);
                    break;
                case NextTrackSetting.Shuffle:
                    _player.NextTrackSelection = NextTrackSetting.AutoStop;
                    _view.SetNextTrackSetting(NextTrackSetting.AutoStop);
                    break;
                case NextTrackSetting.AutoStop:
                    _player.NextTrackSelection = NextTrackSetting.AutoNext;
                    _view.SetNextTrackSetting(NextTrackSetting.AutoNext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SelectTrack(int index)
        {
            _player.SelectTrack(index);
        }

        public void ClearAll()
        {
            _player.ClearTrackList();
            _player.Pause();
            _view.SetProgress(0);
            _view.SetTrackTitle("Unknown track");
            _view.SetTrackDuration(TimeSpan.Zero);
            _view.SetTrackProgress(TimeSpan.Zero);
            _view.ClearAll();
        }
    }
}
