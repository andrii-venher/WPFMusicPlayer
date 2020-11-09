using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMPLib;
using System.Windows.Forms;

namespace WPFMusicPlayer.Model
{
    enum Status
    {
        Undefined,
        Stopped,
        Paused,
        Playing,
        MediaEnded
    }

    public enum NextTrackSetting
    {
        AutoNext,
        Shuffle,
        AutoStop
    }

    class MusicPlayer
    {
        private const double Interval = 10;
        private readonly WindowsMediaPlayer _windowsMediaPlayer;
        private readonly System.Windows.Forms.Timer _timer;
        private readonly List<Track> _trackList;
        public int CurrentTrackIndex { get; private set; }

        public NextTrackSetting NextTrackSelection { get; set; } = NextTrackSetting.AutoNext;

        public delegate Task AsyncEventHandler(object sender, EventArgs e);

        public event Action<object, Status> PlayerStatusChanged;
        public event Action<object, double> ProgressChanged;
        public event Action<object, int> VolumeChanged;
        public event Action<object, Track> TrackSelected;

        public MusicPlayer()
        {
            _windowsMediaPlayer = new WindowsMediaPlayer();

            _windowsMediaPlayer.PlayStateChange += state =>
            {
                if (PlayerStatus == Status.Undefined)
                    return;
                PlayerStatusChanged?.Invoke(this, PlayerStatus);
            };
            _timer = new Timer();
            _timer.Interval = (int) Interval;
            _timer.Tick += (sender, e) =>
            {
                ProgressChanged?.Invoke(this, ProgressRaw);

                if (PlayerStatus == Status.Stopped || PlayerStatus == Status.MediaEnded)
                {
                    if (NextTrackSelection != NextTrackSetting.AutoStop)
                    {
                        SelectNextTrack();
                        return;
                    }
                    (sender as Timer)?.Stop();
                }
            };
            _trackList = new List<Track>();

            Volume = 50;
            Progress = TimeSpan.Zero;
        }

        public void Play()
        {
            if (_trackList.Count == 0)
                return;

            _timer.Start();
            _windowsMediaPlayer.controls.play();
        }

        public void Pause()
        {
            _timer.Stop();
            _windowsMediaPlayer.controls.pause();
        }

        public void Stop()
        {
            _timer.Stop();
            _windowsMediaPlayer.controls.stop();
        }

        public void LoadTrack(string path) => _trackList.Add(new Track(_windowsMediaPlayer.newMedia(path)));

        public void SelectTrack(int index)
        {
            if(_trackList.Count == 0)
                return;

            CurrentTrackIndex = index;

            if (CurrentTrackIndex >= _trackList.Count)
                CurrentTrackIndex = _trackList.Count - 1;
            if (CurrentTrackIndex < 0)
                CurrentTrackIndex = 0;

            if (CurrentTrack.Media != null)
                _windowsMediaPlayer.currentMedia = CurrentTrack.Media;

            ProgressChanged?.Invoke(this, ProgressRaw);
            TrackSelected?.Invoke(this, CurrentTrack);
            VolumeChanged?.Invoke(this, Volume);
            _timer.Start();
        }

        public void SelectNextTrack()
        {
            if(NextTrackSelection == NextTrackSetting.Shuffle)
                SelectTrack(new Random().Next(0, _trackList.Count));
            else
                SelectTrack(CurrentTrackIndex + 1);
        }

        public void SelectPreviousTrack()
        {
            SelectTrack(CurrentTrackIndex - 1);
        }

        public Track CurrentTrack => _trackList.Count > 0 ? _trackList[CurrentTrackIndex] : null;

        public string[] TrackList => _trackList.Select((track) => track.Title).ToArray();

        public TimeSpan Progress
        {
            get => TimeSpan.FromSeconds((int)_windowsMediaPlayer.controls.currentPosition);
            set => _windowsMediaPlayer.controls.currentPosition = value.TotalSeconds;
        }

        public double ProgressRaw
        {
            get => _windowsMediaPlayer.controls.currentPosition;
            set
            {
                if(_trackList.Count == 0)
                    return;

                if (value < 0.0)
                {
                    _windowsMediaPlayer.controls.currentPosition = 0.0;
                }
                else if (value >= CurrentTrack.DurationRaw)
                {
                    _windowsMediaPlayer.controls.currentPosition = CurrentTrack.DurationRaw;
                }
                else if(Math.Abs(_windowsMediaPlayer.controls.currentPosition - value) > (Interval + 100) / 1000)
                {
                    _windowsMediaPlayer.controls.currentPosition = value;
                }
            }
        }

        public int Volume
        {
            get => _windowsMediaPlayer.settings.volume;
            set
            {
                if (value < 0)
                    _windowsMediaPlayer.settings.volume = 0;
                else if (value > 100)
                    _windowsMediaPlayer.settings.volume = 100;
                else
                    _windowsMediaPlayer.settings.volume = value;

                VolumeChanged?.Invoke(this, _windowsMediaPlayer.settings.volume);
            }
        }

        public void ClearTrackList()
        {
            _trackList.Clear();
        }

        public Status PlayerStatus
        {
            get
            {
                switch (_windowsMediaPlayer.playState)
                {
                    case WMPPlayState.wmppsStopped:
                        return Status.Stopped;
                    case WMPPlayState.wmppsPaused:
                        return Status.Paused;
                    case WMPPlayState.wmppsPlaying:
                        return Status.Playing;
                    case WMPPlayState.wmppsMediaEnded:
                        return Status.MediaEnded;
                    default:
                        return Status.Undefined;
                }
            }
        }
    }
}
