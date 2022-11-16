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
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository repository,
                                IMapper mapper,
                                LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }


        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false) {
            try
            {
                var results = await repository.GetAllCampsAsync(includeTalks);
                //    CampModel[] models = mapper.Map<CampModel[]>(results);
                // return Ok(models);
                return mapper.Map<CampModel[]>(results);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                        "Database failer" + e);
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await repository.GetCampAsync(moniker);
                if (result == null) return NotFound();
                return mapper.Map<CampModel>(result);
            }
            catch (Exception e){
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       "Database failer" + e);
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks=false)
        {
            try {
                Console.WriteLine(theDate);
                var result = await repository.GetAllCampsByEventDate(theDate, includeTalks);
                Console.WriteLine(result);
                if (!result.Any()) return NotFound();
                return mapper.Map<CampModel[]>(result);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                   "Database failer" + e);
            }
        }

        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try {
                var camp1 = repository.GetCampAsync(model.Moniker);
                if (camp1 != null) {
                    return BadRequest("Moniker in US");
                }
                var location = linkGenerator.GetPathByAction("Get", "Camps",new { moniker = model.Moniker});
                if (string.IsNullOrWhiteSpace(location)) {
                    return BadRequest("invalid Moniker");
                }
                var camp = mapper.Map<Camp>(model);
                repository.Add(camp);
                if (await repository.SaveChangesAsync())
                {
                    return Created($"api/camps/{camp.Moniker}", mapper.Map<CampModel>(camp));
                }

                return Ok();
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                   "Database failer" + e);
            }
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"no this moniker {moniker}");
                mapper.Map(model, oldCamp);
                if (await repository.SaveChangesAsync()) {
                    return mapper.Map<CampModel>(oldCamp);
                }
                return this.StatusCode(StatusCodes.Status500InternalServerError, "map data error");
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                   "Database failer" + e);
            }
        }
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker) 
        {
            try
            {
                var oldCamp = await repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"no this moniker {moniker}");
                repository.Delete(oldCamp);
                if (await repository.SaveChangesAsync())
                {
                    return Ok();
                }
                return this.StatusCode(StatusCodes.Status500InternalServerError, "map data error");
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                   "Database failer" + e);
            }
        }
    }

}
