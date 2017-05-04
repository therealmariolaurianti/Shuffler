using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using ShufflerPro.Core.Objects;

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
            IsCompleted = false;
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
        public bool IsCompleted { get; set; }

        private async Task PutTaskDelay(TimeSpan time)
        {
            var task = Task.Delay(time, _cancellationToken.Token);
            await task;
            IsCompleted = task.IsCompleted;
        }

        public async Task<bool> PlaySong(Song song)
        {
            try
            {
                using (_audioFileReader = new AudioFileReader(song.Path))
                {
                    if (_outEvent == null)
                    {
                        _outEvent = new WaveOutEvent();
                    }

                    _outEvent.Init(_audioFileReader);
                    _outEvent.Play();

                    try
                    {
                        await PutTaskDelay(_audioFileReader.TotalTime);
                    }
                    catch(Exception ex)
                    {
                        //TODO log error
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