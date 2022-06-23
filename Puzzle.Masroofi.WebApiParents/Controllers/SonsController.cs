using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SonsController : ControllerBase
    {
        private readonly ISonService _sonService;
        public SonsController(ISonService sonService)
        {
            _sonService = sonService;
        }

        /// <summary>
        /// Get Sons list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        /// <response code="200">ATMCard Status is enum: Pending = 1, Rejected = 2, InProgress = 3, Shipping = 4, Received = 5, Active = 6, Deactivated = 7, Lost = 8, Replaced = 9</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SonFilterViewModel model)
        {
            var result = await _sonService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Son By SonId
        /// </summary>
        /// <response code="200">ATMCard Status is enum: Pending = 1, Rejected = 2, InProgress = 3, Shipping = 4, Received = 5, Active = 6, Deactivated = 7, Lost = 8, Replaced = 9</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _sonService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Add new Son
        /// </summary>
        /// <param name="model">
        /// * Must adding son image by send fileBase64 and fileName in newImage property
        /// * Gender is enum: Male = 1, Female = 2
        /// </param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SonInputViewModel model)
        {
            await _sonService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update Son
        /// </summary>
        /// <param name="model">
        /// * If will update image send fileBase64 and fileName in newImage property, if will keep old image send imageUrl property
        /// * Gender is enum: Male = 1, Female = 2
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SonInputViewModel model)
        {
            await _sonService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete Son
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sonService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Activate or Deactivate Son
        /// </summary>
        [HttpPut("change-activation")]
        public async Task<IActionResult> ChangeActivation([FromBody] SonActivationViewModel model)
        {
            var result = await _sonService.ChangeActivationAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Son balance by SonId 
        /// </summary>
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var result = await _sonService.GetBalanceAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
