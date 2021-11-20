using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asaph.WebApi.GetSongDirectors
{
    [Route(Routes.SongDirectors)]
    [ApiController]
    public class GetSongDirectorsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new NotFoundResult();
        }
    }
}
