using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Cities;
using Puzzle.Masroofi.ServiceInterface;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiAdmin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        /// <summary>
        /// Get Cities list by GovernorateId or serach in CityName
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CityFilterViewModel model)
        {            
            var result = await _cityService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get City by CityId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _cityService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
