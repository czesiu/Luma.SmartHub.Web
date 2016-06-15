using System;
using System.Linq;
using Luma.SmartHub.Audio;
using Luma.SmartHub.Audio.Playback;
using Microsoft.AspNet.Mvc;

namespace Luma.SmartHub.Web.Controllers
{
    public class AudioController : Controller
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly IPlaylistProvider _playlistProvider;

        public AudioController(
            IAudioPlayer audioPlayer,
            IPlaylistProvider playlistProvider
        )
        {
            _audioPlayer = audioPlayer;
            _playlistProvider = playlistProvider;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Devices = _audioPlayer.AudioHub.Outputs();
            ViewBag.Playbacks = _audioPlayer.Playbacks;

            return View("Index");
        }

        [HttpPost]
        public ActionResult PlayUrl(string url, string deviceId)
        {
            var device = _audioPlayer.AudioHub.Outputs().Single(c => c.Id == deviceId);

            _audioPlayer.Play(new Uri(url), new[] { device });

            return Index();
        }

        [HttpPost]
        public ActionResult Play(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Play();

            return Index();
        }

        [HttpPost]
        public ActionResult Pause(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Pause();

            return Index();
        }

        [HttpPost]
        public ActionResult Stop(string id)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);

            playback.Stop();

            return Index();
        }

        [HttpPost]
        public ActionResult PlayPlaylist(string url, string deviceId)
        {
            var output = _audioPlayer.AudioHub.Outputs().Single(d => d.Id == deviceId);

            var tracks = _playlistProvider.CreatePlaylist(new Uri(url));

            var playback = new PlaylistPlayback(_audioPlayer.AudioHub, tracks);

            playback.AddOutgoingConnection(output);

            _audioPlayer.AddPlayback(playback);

            playback.Play();

            return Index();
        }

        [HttpPost]
        public ActionResult Prev(string id)
        {
            var playlist = _audioPlayer.Playbacks.OfType<IPlaylistPlayback>().Single(c => c.Id == id);

            playlist.Prev();

            return Index();
        }

        [HttpPost]
        public ActionResult Next(string id)
        {
            var playlist = _audioPlayer.Playbacks.OfType<IPlaylistPlayback>().Single(c => c.Id == id);

            playlist.Next();

            return Index();
        }

        [HttpPost]
        public ActionResult AddOutgoingConnection(string id, string deviceId)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);
            var device = _audioPlayer.AudioHub.Outputs().Single(c => c.Id == deviceId);

            playback.AddOutgoingConnection(device);

            return Index();
        }

        [HttpPost]
        public ActionResult RemoveOutgoingConnection(string id, string deviceId)
        {
            var playback = _audioPlayer.Playbacks.Single(c => c.Id == id);
            var device = _audioPlayer.AudioHub.Outputs().Single(c => c.Id == deviceId);

            playback.RemoveOutgoingConnection(device);

            return Index();
        }
    }
}