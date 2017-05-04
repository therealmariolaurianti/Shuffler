using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NAudio.Wave;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Tasks;

// ReSharper disable AccessToDisposedClosure

namespace ShufflerPro.Core.Workers
{
    public class Player : IDisposable
    {
        private AudioFileReader _audioFileReader;
        private WaveOutEvent _outEvent;

        public Player(WaveOutEvent outEvent, CancellationTokenSource cancellationToken)
        {
            _outEvent = outEvent;
            _cancellationToken = cancellationToken;
        }

        public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
        public bool IsCanceled => _cancellationToken.IsCancellationRequested;

        public void ReInitialize()
        {
            Dispose();

            _outEvent = new WaveOutEvent();
            _cancellationToken = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _outEvent?.Dispose();
            _audioFileReader?.Dispose();
            _cancellationToken?.Dispose();

            _outEvent = null;
            _audioFileReader = null;
            _cancellationToken = null;
        }
        
        public void Cancel()
        {
            _cancellationToken.Cancel();
            ReInitialize();
        }

        private CancellationTokenSource _cancellationToken;
        public bool CompletedSong;

        private async Task PutTaskDelay(TimeSpan time)
        {
            await Task.Delay(time, _cancellationToken.Token);
            CompletedSong = true;
        }

        public async Task<bool> PlaySong(Song song)
        {
            try
            {
                using (_audioFileReader = new AudioFileReader(song.Path))
                {
                    if (_audioFileReader == null || _outEvent == null)
                        return false;

                    _outEvent.Init(_audioFileReader);
                    _outEvent.Play();

                    try
                    {
                        await PutTaskDelay(_audioFileReader.TotalTime);
                    }
                    catch
                    {
                        // ignored
                    }

                    Dispose();
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}