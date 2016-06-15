using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luma.SmartHub.Audio;
using Luma.SmartHub.Audio.Bass;
using Luma.SmartHub.Audio.Playback;
using Microsoft.AspNet.Mvc;

namespace Luma.SmartHub.Web.Controllers
{
    [Route("api/[controller]")]
    public class AudioPlayerController : Controller
    {
        private readonly IAudioHub _audioHub;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IPlaylistProvider _playlistProvider;

        public AudioPlayerController(
            IAudioHub audioHub,
            IAudioPlayer audioPlayer,
            IPlaylistProvider playlistProvider
        )
        {
            _audioHub = audioHub;
            _audioPlayer = audioPlayer;
            _playlistProvider = playlistProvider;
        }

        [HttpGet]
        public IEnumerable Get()
        {
            return _audioPlayer.Playbacks.Select(c => new
            {
                c.Id,
                c.Name,
                c.IsPlaying,
                c.Volume,
                OutgoingConnections = c.OutgoingConnections.Select(o => o.Id)
            });
        }

        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _audioPlayer.Playbacks.Select(c => new
            {
                c.Id,
                c.Name,
                c.IsPlaying,
                c.Volume,
                OutgoingConnections = c.OutgoingConnections.Select(o => o.Id)
            }).SingleOrDefault(c => c.Id == id);
        }

        public class PlayRequest
        {
            public Uri Uri { get; set; }
            public string[] OutgoingConnections { get; set; }
        }

        [HttpPost("play")]
        public void Post([FromBody]PlayRequest request)
        {
            var outputs = request.OutgoingConnections.Select(c => _audioHub.Outputs().Single(d => d.Id == c));

            _audioPlayer.Play(request.Uri, outputs);
        }


        [HttpPut("{id}/play")]
        public void Play(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Play();
        }

        [HttpPut("{id}/pause")]
        public void Pause(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Pause();
        }

        [HttpPut("{id}/stop")]
        public void Stop(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Stop();
        }

        [HttpGet("playlists")]
        public IEnumerable GetPlaylists()
        {
            return _audioPlayer.Playbacks.OfType<IPlaylistPlayback>().Select(c => new
            {
                c.Id,
                c.Name,
                c.IsPlaying,
                c.Volume,
                c.Tracks,
                c.CurrentTrack,
                OutgoingConnections = c.OutgoingConnections.Select(o => o.Id)
            });
        }

        [HttpPost("playlists/play")]
        public void PlayPlaylist([FromBody]PlayRequest request)
        {
            var outputs = request.OutgoingConnections.Select(c => _audioHub.Outputs().Single(d => d.Id == c));

            var tracks = _playlistProvider.CreatePlaylist(request.Uri);

            var playback = new PlaylistPlayback(_audioHub, tracks);

            playback.AddOutgoingConnections(outputs);

            _audioPlayer.AddPlayback(playback);

            playback.Play();
        }

        [HttpPut("playlists/{id}/prev")]
        public void Prev(string id)
        {
            var playlist = _audioPlayer.Playbacks.OfType<IPlaylistPlayback>().Single(c => c.Id == id);

            playlist.Prev();
        }

        [HttpPut("playlists/{id}/next")]
        public void Next(string id)
        {
            var playlist = _audioPlayer.Playbacks.OfType<IPlaylistPlayback>().Single(c => c.Id == id);

            playlist.Next();
        }

        [HttpPut("{id}/volume/{volume}")]
        public void Volume(string id, double volume)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Volume = volume;
        }

        [HttpPut("{id}/position/{position}")]
        public void Position(string id, double position)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Position = position;
        }

        [HttpPut("{id}/outgoingConnections/{deviceId}")]
        public void Put(string id, string deviceId)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);
            var device = _audioHub.Outputs().Single(c => c.Id == deviceId);

            playback.AddOutgoingConnection(device);
        }

        [HttpDelete("{id}/outgoingConnections/{deviceId}")]
        public void Delete(string id, string deviceId)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);
            var device = _audioHub.Outputs().Single(c => c.Id == deviceId);

            playback.RemoveOutgoingConnection(device);
        }
    }
}
