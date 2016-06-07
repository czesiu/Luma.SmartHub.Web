using System.Collections;
using System.Linq;
using Luma.SmartHub.Audio;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Luma.SmartHub.Web.Controllers
{
    [Route("api/[controller]")]
    public class AudioHubController : Controller
    {
        private readonly IAudioHub _audioHub;

        public AudioHubController(IAudioHub audioHub)
        {
            _audioHub = audioHub;
        }

        [HttpGet("devices")]
        public IEnumerable Get()
        {
            return _audioHub.Devices.Select(c => new
            {
                c.Id,
                c.Type,
                c.Name
            });
        }
        
        [HttpPut("devices/{id}/volume/{volume}")]
        public void Put(string id, double volume)
        {
            var device = _audioHub.Outputs().Single(c => c.Id == id);

            device.Volume = volume;
        }
    }
}
