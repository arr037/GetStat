using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NAudio.Wave;
using NetCoreAudio;
using Vlc.DotNet.Wpf;


namespace GetStat.Services
{
    public class MediaPlayerService
    {
        private VlcControl control;
        private Stream music;
        public int Volume { get; set; } = 100;
        public bool IsAutoPlay { get; set; } = true;

        public MediaPlayerService()
        {
            control = new VlcControl();
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            control.SourceProvider.CreatePlayer(libDirectory);

            var mp = Properties.Resources.push2;

            music = new MemoryStream(mp);
            

        }


        public Task Play(bool check = false)
        {
            if (IsAutoPlay && check)
            {
                control.SourceProvider.MediaPlayer.Audio.Volume = Volume;
                control.SourceProvider.MediaPlayer.Play(music);
            }
            return Task.CompletedTask;
        }

        public Task Init()
        {
            Volume = GetStatApp.Default.Volume;
            IsAutoPlay = GetStatApp.Default.IsAutoPlay;
            return Task.CompletedTask;
        }
    }   
}
