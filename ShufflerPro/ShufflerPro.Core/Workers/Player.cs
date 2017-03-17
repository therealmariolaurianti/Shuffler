using System;
using System.Collections.Generic;
using Caliburn.Micro;
using NAudio.Wave;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Task;

// ReSharper disable AccessToDisposedClosure

namespace ShufflerPro.Core.Workers
{
    public class Player
    {
        public Queue<Song> Songs { get; set; }
        private readonly WaveOutEvent _outEvent;
        private AudioFileReader _audioFileReader;
        private readonly IEventAggregator _eventAggregator;

        public Player(WaveOutEvent outEvent, IEventAggregator eventAggregator)
        {
            _outEvent = outEvent;
            _eventAggregator = eventAggregator;
        }

        private void StartPlayer()
        {
            if (Songs == null)
                return;
            if (Songs.Count == 0)
                return;

            var song = Songs.Dequeue();

            using (_audioFileReader = new AudioFileReader(song.Path))
            {
                _outEvent.Init(_audioFileReader);
                _outEvent.PlaybackStopped += delegate { DisposeUsings(_outEvent, _audioFileReader); };
                _outEvent.Play();

                _eventAggregator.PublishOnUIThreadAsync(new NowPlaying(song));

                var songLength = _audioFileReader.TotalTime;
                while (_outEvent.PlaybackState == PlaybackState.Playing)
                {
                    if (_audioFileReader.CurrentTime != songLength)
                        continue;
                    break;
                }

                DisposeUsings(_outEvent, _audioFileReader);
            }

            Play();
        }

        private static void DisposeUsings(IWavePlayer output, IDisposable player)
        {
            output.Stop();
            player.Dispose();
        }

        public void Play()
        {
            StartPlayer();
        }

        public void Stop()
        {
            
        }

        public void Pause()
        {
            
        }

        public void Skip()
        {
            _outEvent.Stop();
        }
    }
}