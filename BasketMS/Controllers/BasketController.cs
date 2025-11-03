using Microsoft.AspNetCore.Mvc;
using BasketMS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasketMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("user/{userName}")]
        public async Task<ActionResult<BasketDto>> GetOrCreateBasketForUser(string userName)
        {
            var basket = await _basketService.GetOrCreateBasketForUserAsync(userName);
            if (basket == null)
                return NotFound();
            return Ok(basket);
        }

        [HttpPost("add-item")]
        public async Task<ActionResult<BasketDto>> AddItemToBasket([FromBody] AddItemDto dto)
        {
            var basket = await _basketService.AddItemToBasketAsync(dto.Username, dto.CatalogItemId, dto.Price, dto.Quantity);
            return Ok(basket);
        }

        [HttpPost("set-quantities")]
        public async Task<ActionResult<BasketDto>> SetQuantities([FromBody] SetQuantitiesDto dto)
        {
            var basket = await _basketService.SetQuantitiesAsync(dto.username, dto.Quantities);
            return Ok(basket);
        }

        [HttpDelete("{basketId}")]
        public async Task<IActionResult> DeleteBasket(int basketId)
        {
            await _basketService.DeleteBasketAsync(basketId);
            return Ok();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferBasket([FromBody] TransferBasketDto dto)
        {
            await _basketService.TransferBasketAsync(dto.AnonymousId, dto.UserName);
            return Ok();
        }

        [HttpGet("{username}/item-count")]
        public async Task<ActionResult<int>> CountTotalBasketItems(string username)
        {
            var count = await _basketService.CountTotalBasketItemsAsync(username);
            return Ok(count);
        }
    }

    // DTOs (should match those used in your client and service)
    public record AddItemDto(string Username, int CatalogItemId, decimal Price, int Quantity = 1);
    public record SetQuantitiesDto(string username, Dictionary<string, int> Quantities);
    public record TransferBasketDto(string AnonymousId, string UserName);
    public record BasketDto(int Id, string BuyerId, List<BasketItemDto> Items);
    public record BasketItemDto(int Id, int CatalogItemId, decimal UnitPrice, int Quantity);
}
