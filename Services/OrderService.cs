using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
using Domain.Entities.PaymentEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal sealed class OrderService : IOrderService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public OrderService(IUnitOFWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<OrderResultDTO> CreateGuestOrderAsync(string basketId, GuestCheckoutRequestDTO checkoutDto)
        {

            Console.WriteLine("FILE = " + checkoutDto.PaymentProofFile?.FileName);
            Console.WriteLine("PayerPhone = " + checkoutDto.PayerPhone);

            // Get basket with items
            var spec = new BasketWithItemsSpecifications(basketId);
            var basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);

            if (basket == null)
                throw new BasketNotFoundException(basketId);

            if (!basket.Items.Any())
                throw new ValidationException(new[] { "Cannot create order from empty basket" });

            // Validate payment method
            var paymentMethod = (Domain.Entities.PaymentEntities.PaymentMethod)checkoutDto.PaymentMethodId;
            if (!Enum.IsDefined(typeof(Domain.Entities.PaymentEntities.PaymentMethod), paymentMethod))
                throw new ValidationException(new[] { "Invalid payment method" });

            // Validate payment proof for non-COD methods
            if (paymentMethod != Domain.Entities.PaymentEntities.PaymentMethod.CashOnDelivery)
            {
                if (checkoutDto.PaymentProofFile == null)
                    throw new ValidationException(new[] { "Payment proof is required for wallet/InstaPay payments" });
                
                if (string.IsNullOrWhiteSpace(checkoutDto.PayerPhone))
                    throw new ValidationException(new[] { "Payer phone is required when uploading payment proof" });
            }

            // Get shipping cost for governorate
            var governorateSpec = new GovernorateShippingSpecifications(checkoutDto.Governorate);
            var shippingInfo = await _unitOfWork.GetRepository<GovernorateShippingPrice, int>().GetAsync(governorateSpec);
            
            if (shippingInfo == null)
                throw new ValidationException(new[] { $"Shipping not available for governorate: {checkoutDto.Governorate}" });

            // Calculate prices
            var subTotal = basket.TotalPrice;
            var shippingCost = shippingInfo.ShippingPrice;
            var totalPrice = subTotal + shippingCost;
            var estimatedDelivery = DateTime.UtcNow.AddDays(shippingInfo.DeliveryDays);

            // Create order
            var order = new Order
            {
                BasketId = basketId,
                UserId = null,
                OrderToken = Guid.NewGuid().ToString("N"), // Generate token for guest access
                CustomerName = checkoutDto.CustomerName,
                CustomerEmail = checkoutDto.CustomerEmail,
                CustomerPhone = checkoutDto.CustomerPhone,
                ShippingAddress = checkoutDto.ShippingAddress,
                Governorate = checkoutDto.Governorate,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TotalPrice = totalPrice,
                EstimatedDeliveryDate = estimatedDelivery,
                PaymentMethod = paymentMethod,
                PaymentStatus = Domain.Entities.PaymentEntities.PaymentStatus.Pending,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Create order items (snapshot of basket items with product details)
            foreach (var item in basket.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ProductPictureUrl = item.Product.PictureUrl,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CostPrice = item.Product.CostPrice, // Save cost price for profit calculation
                    Color = item.Color,
                    Size = item.Size
                };
                await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
            }

            // Delete basket after order creation
            _unitOfWork.GetRepository<Basket, string>().Delete(basket);

            await _unitOfWork.SaveChangesAsync();

            // Upload payment proof if provided
            if (checkoutDto.PaymentProofFile != null && !string.IsNullOrWhiteSpace(checkoutDto.PayerPhone))
            {
                try
                {
                    // Upload file to R2
                    var fileUrl = await _fileStorageService.UploadAsync(checkoutDto.PaymentProofFile, $"payments/{order.Id}");

                    // Create payment proof record
                    var proof = new OrderPaymentProof
                    {
                        OrderId = order.Id,
                        FileUrl = fileUrl,
                        PayerPhone = checkoutDto.PayerPhone,
                        UploadedByUserId = null, // Guest order
                        UploadedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.GetRepository<OrderPaymentProof, int>().AddAsync(proof);
                    await _unitOfWork.SaveChangesAsync();

                    // Link proof to order
                    order.PaymentProofId = proof.Id;
                    _unitOfWork.GetRepository<Order, int>().Update(order);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log detailed error information
                    Console.WriteLine($"[ERROR] File upload failed!");
                    Console.WriteLine($"[ERROR] Exception Type: {ex.GetType().Name}");
                    Console.WriteLine($"[ERROR] Message: {ex.Message}");
                    Console.WriteLine($"[ERROR] Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[ERROR] Inner Exception: {ex.InnerException.Message}");
                        Console.WriteLine($"[ERROR] Inner Stack Trace: {ex.InnerException.StackTrace}");
                    }
                    
                    // If file upload fails, rollback and delete the file if it was uploaded
                    throw new ValidationException(new[] { $"Failed to upload payment proof: {ex.Message}. Inner: {ex.InnerException?.Message}" });
                }
            }

            // Reload order with items to return
            var orderSpec = new OrderWithItemsSpecifications(order.Id);
            var createdOrder = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderSpec);

            return _mapper.Map<OrderResultDTO>(createdOrder);
        }

        public async Task<OrderResultDTO> CreateUserOrderAsync(string userId, GuestCheckoutRequestDTO checkoutDto)
        {
            // Get user's basket with items
            var spec = new BasketWithItemsSpecifications(userId, byUserId: true);
            var basket = await _unitOfWork.GetRepository<Basket, string>().GetAsync(spec);

            if (basket == null)
                throw new BasketNotFoundException($"User {userId} basket");

            if (!basket.Items.Any())
                throw new ValidationException(new[] { "Cannot create order from empty basket" });

            // Validate payment method
            var paymentMethod = (PaymentMethod)checkoutDto.PaymentMethodId;
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
                throw new ValidationException(new[] { "Invalid payment method" });

            // Validate payment proof for non-COD methods
            if (paymentMethod != PaymentMethod.CashOnDelivery)
            {
                if (checkoutDto.PaymentProofFile == null)
                    throw new ValidationException(new[] { "Payment proof is required for wallet/InstaPay payments" });
                
                if (string.IsNullOrWhiteSpace(checkoutDto.PayerPhone))
                    throw new ValidationException(new[] { "Payer phone is required when uploading payment proof" });
            }

            // Get shipping cost for governorate
            var governorateSpec = new GovernorateShippingSpecifications(checkoutDto.Governorate);
            var shippingInfo = await _unitOfWork.GetRepository<GovernorateShippingPrice, int>().GetAsync(governorateSpec);

            if (shippingInfo == null)
                throw new ValidationException(new[] { $"Shipping not available for governorate: {checkoutDto.Governorate}" });

            // Calculate prices
            var subTotal = basket.TotalPrice;
            var shippingCost = shippingInfo.ShippingPrice;
            var totalPrice = subTotal + shippingCost;
            var estimatedDelivery = DateTime.UtcNow.AddDays(shippingInfo.DeliveryDays);

            // Create order
            var order = new Order
            {
                BasketId = basket.Id,
                UserId = userId,
                OrderToken = Guid.NewGuid().ToString("N"), // Generate token for order tracking
                CustomerName = checkoutDto.CustomerName,
                CustomerEmail = checkoutDto.CustomerEmail,
                CustomerPhone = checkoutDto.CustomerPhone,
                ShippingAddress = checkoutDto.ShippingAddress,
                Governorate = checkoutDto.Governorate,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TotalPrice = totalPrice,
                EstimatedDeliveryDate = estimatedDelivery,
                PaymentMethod = paymentMethod,
                PaymentStatus = Domain.Entities.PaymentEntities.PaymentStatus.Pending,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Order, int>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Create order items (snapshot of basket items with product details)
            foreach (var item in basket.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ProductPictureUrl = item.Product.PictureUrl,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CostPrice = item.Product.CostPrice, // Save cost price for profit calculation
                    Color = item.Color,
                    Size = item.Size
                };
                await _unitOfWork.GetRepository<OrderItem, int>().AddAsync(orderItem);
            }

            // Delete basket after order creation
            _unitOfWork.GetRepository<Basket, string>().Delete(basket);

            await _unitOfWork.SaveChangesAsync();

            // Upload payment proof if provided
            if (checkoutDto.PaymentProofFile != null && !string.IsNullOrWhiteSpace(checkoutDto.PayerPhone))
            {
                try
                {
                    // Upload file to R2
                    var fileUrl = await _fileStorageService.UploadAsync(checkoutDto.PaymentProofFile, $"payments/{order.Id}");

                    // Create payment proof record
                    var proof = new OrderPaymentProof
                    {
                        OrderId = order.Id,
                        FileUrl = fileUrl,
                        PayerPhone = checkoutDto.PayerPhone,
                        UploadedByUserId = userId,
                        UploadedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.GetRepository<OrderPaymentProof, int>().AddAsync(proof);
                    await _unitOfWork.SaveChangesAsync();

                    // Link proof to order
                    order.PaymentProofId = proof.Id;
                    _unitOfWork.GetRepository<Order, int>().Update(order);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // If file upload fails, rollback and delete the file if it was uploaded
                    throw new ValidationException(new[] { $"Failed to upload payment proof: {ex.Message}" });
                }
            }

            // Reload order with items to return
            var orderSpec = new OrderWithItemsSpecifications(order.Id);
            var createdOrder = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderSpec);

            return _mapper.Map<OrderResultDTO>(createdOrder);
        }

        public async Task<OrderResultDTO> GetOrderByIdAsync(int orderId)
        {
            var spec = new OrderWithItemsSpecifications(orderId);
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(spec);

            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            return _mapper.Map<OrderResultDTO>(order);
        }

        public async Task<IEnumerable<OrderResultDTO>> GetUserOrdersAsync(string userId)
        {
            var spec = new OrderWithItemsSpecifications(userId, byUserId: true);
            var orders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync(spec);

            return _mapper.Map<IEnumerable<OrderResultDTO>>(orders);
        }

        public async Task<IEnumerable<OrderResultDTO>> GetAllOrdersAsync()
        {
            var spec = new OrderWithItemsSpecifications();
            var orders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync(spec);

            return _mapper.Map<IEnumerable<OrderResultDTO>>(orders);
        }

        public async Task<OrderResultDTO> UpdateOrderStatusAsync(int orderId, int status, string? trackingNumber)
        {
            var spec = new OrderWithItemsSpecifications(orderId);
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(spec);

            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            var newStatus = (OrderStatus)status;
            if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
                throw new ValidationException(new[] { "Invalid order status" });

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Update tracking number if provided
            if (!string.IsNullOrWhiteSpace(trackingNumber))
                order.TrackingNumber = trackingNumber;

            // Set timestamps based on status
            if (newStatus == OrderStatus.Shipping && order.ShippedAt == null)
                order.ShippedAt = DateTime.UtcNow;
            
            if (newStatus == OrderStatus.Delivered && order.DeliveredAt == null)
                order.DeliveredAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Order, int>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            // Reload to get updated data
            var updatedOrder = await _unitOfWork.GetRepository<Order, int>().GetAsync(spec);
            return _mapper.Map<OrderResultDTO>(updatedOrder);
        }

        public async Task<IEnumerable<GovernorateShippingDTO>> GetAllGovernorateShippingAsync()
        {
            var spec = new GovernorateShippingSpecifications();
            var governorates = await _unitOfWork.GetRepository<GovernorateShippingPrice, int>().GetAllAsync(spec);

            return _mapper.Map<IEnumerable<GovernorateShippingDTO>>(governorates);
        }

        public async Task<GovernorateShippingDTO> UpdateGovernorateShippingAsync(int id, UpdateGovernorateShippingDTO dto)
        {
            var governorate = await _unitOfWork.GetRepository<GovernorateShippingPrice, int>().GetAsync(id);

            if (governorate == null)
            {
                // Create new governorate shipping if doesn't exist
                governorate = new GovernorateShippingPrice
                {
                    GovernorateName = dto.GovernorateName,
                    ShippingPrice = dto.ShippingPrice,
                    DeliveryDays = dto.DeliveryDays,
                    IsActive = dto.IsActive
                };
                await _unitOfWork.GetRepository<GovernorateShippingPrice, int>().AddAsync(governorate);
            }
            else
            {
                // Update existing
                governorate.GovernorateName = dto.GovernorateName;
                governorate.ShippingPrice = dto.ShippingPrice;
                governorate.DeliveryDays = dto.DeliveryDays;
                governorate.IsActive = dto.IsActive;
                _unitOfWork.GetRepository<GovernorateShippingPrice, int>().Update(governorate);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GovernorateShippingDTO>(governorate);
        }
    }
}
