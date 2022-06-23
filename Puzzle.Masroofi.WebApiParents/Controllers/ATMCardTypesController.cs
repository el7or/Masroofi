using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTypes;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ATMCardTypesController : ControllerBase
    {
        private readonly IATMCardTypeService _atmCardTypeService;
        public ATMCardTypesController(IATMCardTypeService atmCardTypeService)
        {
            _atmCardTypeService = atmCardTypeService;
        }

        /// <summary>
        /// Get ATMCardTypes list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ATMCardTypeFilterViewModel model)
        {
            var result = await _atmCardTypeService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get ATMCardType By ATMCardTypeId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _atmCardTypeService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Add new ATMCardType
        /// </summary>
        /// <param name="model">
        /// * Must adding FrontImage by send fileBase64 and fileName in newFrontImage property
        /// * Must adding BackImage by send fileBase64 and fileName in newBackImage property
        /// </param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ATMCardTypeInputViewModel model)
        {
            await _atmCardTypeService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update ATMCardType
        /// </summary>
        /// <param name="model">
        /// * If will update any image send fileBase64 and fileName in newImage property, if will keep old image send imageUrl property
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ATMCardTypeInputViewModel model)
        {
            await _atmCardTypeService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete ATMCardType
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _atmCardTypeService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
