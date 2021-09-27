using Asaph.Core.UseCases.AddSongDirector;
using Asaph.WebApi.Models;
using Hydra.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Asaph.WebApi.AddSongDirector
{
    [Route("song-directors")]
    [ApiController]
    public class AddSongDirectorController : ControllerBase
    {
        [HttpPost]
        [Operation(
            typeof(Collection<SongDirectorGrandmasterViewModel>),
            Method = Method.Post,
            Title = "Add song director"
        )]
        [Authorize]
        [EnableCors("AllowAll")]
        public ActionResult<AddSongDirectorResponse> Post(
            AddSongDirectorRequest addSongDirectorRequest)
        {
            throw new NotImplementedException();
        }
    }
}
