using System;
using System.Collections.Generic;
using NAudio.Wave;
using ShufflerPro.Core.Objects;
// ReSharper disable AccessToDisposedClosure

namespace ShufflerPro.Core.Workers
{
    public class Player
    {
        private readonly Queue<Song> _songs;

        public Player(Queue<Song> songs)
        {
            _songs = songs;
        }

        private void StartPlayer()
        {
            if (_songs.Count == 0)
                return;

            var song = _songs.Dequeue();

            using (var output = new WaveOutEvent())
            using (var player = new AudioFileReader(song.Path))
            {
                output.Init(player);
                output.PlaybackStopped += (send, evn) =>
                {
                    Play();
                    DisposeUsings(output, player);
                };
                output.Play();

                var songLength = player.TotalTime;

                while (output.PlaybackState == PlaybackState.Playing)
                {
                    if (player.CurrentTime != songLength)
                        continue;
                    break;
                }

                DisposeUsings(output, player);
            }

            Play();
        }

        private static void DisposeUsings(WaveOutEvent output, AudioFileReader player)
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
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
    }
}