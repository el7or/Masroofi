using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Vendors;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiVendors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        /// <summary>
        /// Get Vendors list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] VendorFilterViewModel model)
        {
            var result = await _vendorService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Vendor By VendorId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _vendorService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Add new Vendor
        /// </summary>
        /// <param name="model">
        /// * For adding parent image send fileBase64 and fileName in newImage property
        /// </param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VendorInputViewModel model)
        {
            await _vendorService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update Vendor
        /// </summary>
        /// <param name="model">
        /// * If will update image send fileBase64 and fileName in newImage property, if will keep old image send imageUrl property
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VendorInputViewModel model)
        {
            await _vendorService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete Vendor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _vendorService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Vendor balance by VendorId 
        /// </summary>
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var result = await _vendorService.GetBalanceAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
