using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IOrderService
    {
        Task<OrderResultDTO> CreateGuestOrderAsync(string basketId, GuestCheckoutRequestDTO checkoutDto);
        Task<OrderResultDTO> CreateUserOrderAsync(string userId, GuestCheckoutRequestDTO checkoutDto);
        Task<OrderResultDTO> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderResultDTO>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<OrderResultDTO>> GetAllOrdersAsync();
        Task<OrderResultDTO> UpdateOrderStatusAsync(int orderId, int status, string? trackingNumber);
        Task<IEnumerable<GovernorateShippingDTO>> GetAllGovernorateShippingAsync();
        Task<GovernorateShippingDTO> UpdateGovernorateShippingAsync(int id, UpdateGovernorateShippingDTO dto);
    }
}
