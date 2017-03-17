using System;
using System.Collections.Generic;
using Caliburn.Micro;
using NAudio.Wave;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Task;

// ReSharper disable AccessToDisposedClosure

namespace ShufflerPro.Core.Workers
{
    public class Player : IDisposable
    {
        public Queue<Song> Songs { get; set; }
        private WaveOutEvent _outEvent;
        private AudioFileReader _audioFileReader;
        private readonly IEventAggregator _eventAggregator;

        public Player(WaveOutEvent outEvent, IEventAggregator eventAggregator)
        {
            _outEvent = outEvent;
            _eventAggregator = eventAggregator;
        }

        public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;    

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
                _outEvent.PlaybackStopped += delegate { ((IDisposable) _audioFileReader)?.Dispose(); };

                _outEvent.Play();

                var songLength = _audioFileReader.TotalTime;
                while (_outEvent.PlaybackState == PlaybackState.Playing)
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
            StartPlayer();
        }

        public void Stop()
        {
            _outEvent.Stop();
        }

        public void Pause()
        {
            
        }

        public void Skip()
        {
            Stop();
        }

        public void Dispose()
        {
            _outEvent?.Dispose();
            _audioFileReader?.Dispose();

            _outEvent = null;
            _audioFileReader = null;
        }
    }
}