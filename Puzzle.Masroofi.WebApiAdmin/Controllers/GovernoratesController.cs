using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Governorates;
using Puzzle.Masroofi.ServiceInterface;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiAdmin.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GovernoratesController : ControllerBase
    {
        private readonly IGovernorateService _governorateService;

        public GovernoratesController(IGovernorateService governorateService)
        {
            _governorateService = governorateService;
        }

        /// <summary>
        /// Get Governorates list by serach in GovernorateName
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GovernorateFilterViewModel model)
        {
            var result = await _governorateService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Governorate by GovernorateId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _governorateService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
