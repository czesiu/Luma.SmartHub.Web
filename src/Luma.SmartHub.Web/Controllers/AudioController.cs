using System;
using System.Linq;
using Luma.SmartHub.Audio;
using Microsoft.AspNet.Mvc;

namespace Luma.SmartHub.Web.Controllers
{
    public class AudioController : Controller
    {
        private readonly IAudioPlayer _audioPlayer;

        public AudioController(IAudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
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