using System;
using System.IO;
using WMPLib;

namespace WPFMusicPlayer.Model
{
    class Track
    {
        public IWMPMedia Media { get; private set; }
        public string Title { get; private set; }
        public string SRC { get; private set; }
        public TimeSpan Duration => TimeSpan.FromSeconds(DurationRaw);
        public double DurationRaw { get; private set; }

        public Track(IWMPMedia media)
        {
            Media = media;
            SRC = media.sourceURL;
            Title = Path.GetFileNameWithoutExtension(SRC);
            DurationRaw = media.duration;
        }
    }
}
