using System;
using System.Collections.Generic;
using System.Threading;
using Caliburn.Micro;
using NAudio.Wave;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Task;

// ReSharper disable AccessToDisposedClosure

namespace ShufflerPro.Core.Workers
{
    public class Player : IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private AudioFileReader _audioFileReader;
        private WaveOutEvent _outEvent;

        public Player(WaveOutEvent outEvent, IEventAggregator eventAggregator)
        {
            _outEvent = outEvent;
            _eventAggregator = eventAggregator;
        }

        public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
        public Queue<Song> Songs { get; set; }

        public void Dispose()
        {
            _outEvent?.Dispose();
            _audioFileReader?.Dispose();

            _outEvent = null;
            _audioFileReader = null;
        }

        private void StartPlayer()
        {
            if (Songs == null)
                return;
            if (Songs.Count == 0)
                return;

            var song = Songs.Dequeue();
            _eventAggregator.PublishOnUIThreadAsync(new NowPlaying(song));

            using (_audioFileReader = new AudioFileReader(song.Path))
            {
                if (_audioFileReader == null || _outEvent == null)
                    return;

                _outEvent.Init(_audioFileReader);
                _outEvent.PlaybackStopped += delegate {((IDisposable) _audioFileReader)?.Dispose();};
                _outEvent.Play();

                var songLength = _audioFileReader.TotalTime;
                while (Playing)
                {
                    if (_audioFileReader.CurrentTime != songLength)
                        continue;
                    break;
                }

                ((IDisposable) _audioFileReader)?.Dispose();
            }

            Play();
        }

        public void Play()
        {
            Stop();
            StartPlayer();
        }

        public void Stop()
        {
            _outEvent?.Stop();
        }

        public void Pause()
        {
            _outEvent?.Pause();
        }

        public void Skip()
        {
            Stop();
        }
    }
}