using System;
using WPFMusicPlayer.Model;

namespace WPFMusicPlayer
{
    interface IMPView
    {
        void AddToTrackList(string[] trackList);
        void ClearAll();
        void SetTrackTitle(string title);
        void SetTrackProgress(TimeSpan progress);
        void SetTrackDuration(TimeSpan duration);
        void SetVolume(int volume);
        void SetProgress(int progress);
        void SetMaximumProgress(double maximumProgress);
        void SetPlay();
        void SetPause();
        void SetSelectedTrack(string title);
        void SetNextTrackSetting(NextTrackSetting setting);
    }
}
