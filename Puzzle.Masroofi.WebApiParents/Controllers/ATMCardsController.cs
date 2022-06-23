using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCards;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ATMCardsController : ControllerBase
    {
        private readonly IATMCardService _atmCardService;
       
        public ATMCardsController(IATMCardService atmCardService)
        {
            _atmCardService = atmCardService;
         
        }

        /// <summary>
        /// Get ATMCards list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        /// <response code="200">Status is enum: Pending = 1, Rejected = 2, InProgress = 3, Shipping = 4, Received = 5, Active = 6, Deactivated = 7, Lost = 8, Replaced = 9</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ATMCardFilterViewModel model)
        {
            var result = await _atmCardService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }
        
        /// <summary>
        /// Get ATMCard By ATMCardId
        /// </summary>
        /// <response code="200">Status is enum: Pending = 1, Rejected = 2, InProgress = 3, Shipping = 4, Received = 5, Active = 6, Deactivated = 7, Lost = 8, Replaced = 9</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _atmCardService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get ATMCard Sons Details for mobile pos By CardId
        /// </summary>
        [HttpGet("GetByCardId/{CardId}")]
        public async Task<IActionResult> GetByCardId(long CardId)
        {
            var result = await _atmCardService.GetByCardIdAsync(CardId);
                return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Request new AtmCard for Son
        /// </summary>
        /// <param name="model">
        /// * Son whom will add ATMCard to him must have at least one image and have not another active ATMCard.
        /// * If sent Password, make it encrypted for 3 digits.
        /// * If sent ShortNumber, make it 6 digits.
        /// * If sent CardNumber, make it 16 digits.
        /// * If sent SecurityCode, make it 3 digits.
        /// </param>
        /// <response code="200">Status will be automatically "Pending"</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ATMCardInputViewModel model)
        {
            await _atmCardService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update AtmCard
        /// </summary>
        /// <param name="model">
        /// * If sent Password, make it encrypted for 3 digits.
        /// * If sent ShortNumber, make it 6 digits.
        /// * If sent CardNumber, make it 16 digits.
        /// * If sent SecurityCode, make it 3 digits.
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ATMCardInputViewModel model)
        {
            await _atmCardService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete AtmCard
        /// </summary>
        /// <response code="200">Status will be automatically "Deactivated"</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _atmCardService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Update ATMCard Password
        /// </summary>
        /// <param name="model">
        /// * Password must be encrypted for 3 digits.
        /// </param>
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ATMCardPasswordViewModel model)
        {
            var result = await _atmCardService.UpdatePasswordAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Update ATMCard Status
        /// </summary>
        /// <param name="model">
        /// * Status is enum: Pending = 1, Rejected = 2, InProgress = 3, Shipping = 4, Received = 5, Active = 6, Deactivated = 7, Lost = 8, Replaced = 9
        /// * RejectedReason is required only if Status = Rejected.
        /// * Password encrypted for 3 digits is required only if Status = Active.
        /// </param>
        [HttpPut("update-status")]
        public async Task<ActionResult> UpdateStatusAsync([FromBody] ATMCardStatusViewModel model)
        {
            var result = await _atmCardService.UpdateStatusAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Lost current ATMCard and request new one
        /// </summary>
        /// <param name="model">
        /// * If sent Password, make it encrypted for 3 digits.
        /// * If sent ShortNumber, make it 6 digits.
        /// * If sent CardNumber, make it 16 digits.
        /// * If sent SecurityCode, make it 3 digits.
        /// </param>
        /// <response code="200">Status for old ATMCard will be automatically "Lost" and return New ATMCard data.</response>
        [HttpPut("LostATMCard")]
        public async Task<IActionResult> LostATMCard([FromBody] LostATMCardViewModel model)
        {
            await _atmCardService.LostATMCardAsync(model);
            return Ok(new PuzzleApiResponse(model.NewATMCard));
        }

        /// <summary>
        /// Replace current ATMCard by new one
        /// </summary>
        /// <param name="model">
        /// * If sent Password, make it encrypted for 3 digits.
        /// * If sent ShortNumber, make it 6 digits.
        /// * If sent CardNumber, make it 16 digits.
        /// * If sent SecurityCode, make it 3 digits.
        /// </param>
        /// <response code="200">Status for old ATMCard will be automatically "Replaced" and return New ATMCard data.</response>
        [HttpPut("ReplacedATMCard")]
        public async Task<IActionResult> ReplacedATMCard([FromBody] ReplacedATMCardViewModel model)
        {
            await _atmCardService.ReplacedATMCardAsync(model);
            return Ok(new PuzzleApiResponse(model.NewATMCard));
        }

        /// <summary>
        /// Get ATMCard balance by ATMCardId 
        /// </summary>
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var result = await _atmCardService.GetBalanceAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get ATMCard Status
        /// </summary>
        [HttpGet("AtmCardStatus")]
        public IActionResult GetAtmCardStatus()
        {
            var result = _atmCardService.GetAtmCardStatus();
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
