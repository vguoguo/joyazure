using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkgenerator;

        public TalksController(ICampRepository repository,
                                IMapper mapper,
                                LinkGenerator linkgenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkgenerator = linkgenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker) 
        {
            try
            {
                var talks = await repository.GetTalksByMonikerAsync(moniker);
                return mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                  "Database failer" + e);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> get(string moniker, int id) 
        {
            try
            {
                var result = await repository.GetTalkByMonikerAsync(moniker, id);
                if (result == null) return NotFound();
                return mapper.Map<TalkModel>(result);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       "Database failer" + e);
            }
        }
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                var talk = mapper.Map<Talk>(model);
                talk.Camp = camp;
                var speaker = await repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker could not be found");
                talk.Speaker = speaker;
                repository.Add(talk);
                if (await repository.SaveChangesAsync())
                {
                    var url = linkgenerator.GetPathByAction(HttpContext,
                        "Get",
                        values: new { moniker, id = talk.TalkId });
                    return Created(url, mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Fail to save new talk");
                }
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       "Database failer" + e);
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model) 
        {
            try
            {
                var talk = await repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return NotFound("couldn;t find the talk");
                mapper.Map(model, talk);
               
                if (model.Speaker != null) {
                    var speaker = await repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null) {
                        talk.Speaker = speaker;
                    }
                }

                if (await repository.SaveChangesAsync())
                {
                    return mapper.Map<TalkModel>(talk); ;
                }
                else {
                    return BadRequest("Failed to update ");
                }
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       "Database failer" + e);
            }
        }
    }
}
