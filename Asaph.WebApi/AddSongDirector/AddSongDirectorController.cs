using Asaph.Core.UseCases.AddSongDirector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asaph.WebApi.AddSongDirector
{
    [Route("song-directors")]
    [ApiController]
    public class AddSongDirectorController : ControllerBase
    {
        [HttpPost]
        public ActionResult<AddSongDirectorRequest> Post(
            AddSongDirectorRequest addSongDirectorRequest)
        {
            throw new NotImplementedException();
        }
    }
}
