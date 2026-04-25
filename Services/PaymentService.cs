using AutoMapper;
using Domain.Contracts;
using Domain.Entities.OrderEntities;
using Domain.Entities.PaymentEntities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Services.Abstractions;
using Services.Specifications;
using Shared.PaymentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    internal sealed class PaymentService : IPaymentService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;

        public PaymentService(IUnitOFWork unitOfWork, IMapper mapper, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<PaymentProofResultDto> UploadPaymentProofAsync(int orderId, string? userId, string? basketId, IFormFile file, string payerPhone)
        {
            // Get order
            var orderSpec = new OrderWithItemsSpecifications(orderId);
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderSpec);

            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            // Validate ownership (user order OR guest order via basketId)
            if (!string.IsNullOrEmpty(order.UserId))
            {
                // User order - must match userId
                if (order.UserId != userId)
                    throw new UnAuthorizedException("You are not authorized to upload payment proof for this order");
            }
            else
            {
                // Guest order - must match basketId
                if (order.BasketId != basketId)
                    throw new UnAuthorizedException("You are not authorized to upload payment proof for this order");
            }

            // Check if order already has payment proof
            if (order.PaymentProofId.HasValue)
                throw new ValidationException(new[] { "This order already has a payment proof uploaded" });

            // Upload file to R2
            string fileUrl;
            try
            {
                fileUrl = await _fileStorage.UploadAsync(file, $"payments/{orderId}");
            }
            catch (Exception ex)
            {
                throw new ValidationException(new[] { $"Failed to upload file: {ex.Message}" });
            }

            // Create payment proof record
            var proof = new OrderPaymentProof
            {
                OrderId = orderId,
                FileUrl = fileUrl,
                PayerPhone = payerPhone,
                UploadedByUserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.GetRepository<OrderPaymentProof, int>().AddAsync(proof);
                await _unitOfWork.SaveChangesAsync();

                // Update order
                order.PaymentProofId = proof.Id;
                order.PaymentStatus = PaymentStatus.Pending;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Order, int>().Update(order);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<PaymentProofResultDto>(proof);
            }
            catch (Exception)
            {
                // Rollback: delete uploaded file
                await _fileStorage.DeleteAsync(fileUrl);
                throw;
            }
        }

        public async Task ApprovePaymentAsync(int orderId, string adminId)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            // Check current status to avoid race conditions
            if (order.PaymentStatus == PaymentStatus.Confirmed)
                throw new ValidationException(new[] { "Payment has already been approved" });

            if (!order.PaymentProofId.HasValue)
                throw new ValidationException(new[] { "No payment proof found for this order" });

            // Get proof and update
            var proof = await _unitOfWork.GetRepository<OrderPaymentProof, int>().GetAsync(order.PaymentProofId.Value);
            if (proof == null)
                throw new PaymentNotFoundException(nameof(OrderPaymentProof), order.PaymentProofId.Value.ToString());

            proof.ApprovedAt = DateTime.UtcNow;
            proof.ApprovedByAdminId = adminId;
            _unitOfWork.GetRepository<OrderPaymentProof, int>().Update(proof);

            order.PaymentStatus = PaymentStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectPaymentAsync(int orderId, string adminId, string? rejectionReason)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            if (order.PaymentStatus == PaymentStatus.Rejected)
                throw new ValidationException(new[] { "Payment has already been rejected" });

            if (!order.PaymentProofId.HasValue)
                throw new ValidationException(new[] { "No payment proof found for this order" });

            var proof = await _unitOfWork.GetRepository<OrderPaymentProof, int>().GetAsync(order.PaymentProofId.Value);
            if (proof == null)
                throw new PaymentNotFoundException(nameof(OrderPaymentProof), order.PaymentProofId.Value.ToString());

            proof.RejectedAt = DateTime.UtcNow;
            proof.RejectedByAdminId = adminId;
            proof.RejectionReason = rejectionReason;
            _unitOfWork.GetRepository<OrderPaymentProof, int>().Update(proof);

            order.PaymentStatus = PaymentStatus.Rejected;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Order, int>().Update(order);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync()
        {
            var methods = new List<PaymentMethodDto>
            {
                new PaymentMethodDto
                {
                    Id = "CashOnDelivery",
                    Name = "Cash on Delivery",
                    PhoneNumber = null,
                    IsActive = true
                }
            };

            // Get wallet and instapay settings
            var settings = await _unitOfWork.GetRepository<PaymentSettings, int>().GetAllAsync();
            
            foreach (var setting in settings.Where(s => s.IsActive))
            {
                methods.Add(new PaymentMethodDto
                {
                    Id = setting.Method.ToString(),
                    Name = setting.DisplayName,
                    PhoneNumber = setting.PhoneNumber,
                    IsActive = setting.IsActive
                });
            }

            return methods;
        }

        public async Task UpdatePaymentSettingAsync(string method, string phoneNumber)
        {
            if (!Enum.TryParse<PaymentMethod>(method, true, out var paymentMethod))
                throw new ValidationException(new[] { "Invalid payment method" });

            if (paymentMethod == PaymentMethod.CashOnDelivery)
                throw new ValidationException(new[] { "Cannot update settings for Cash on Delivery" });

            var settings = await _unitOfWork.GetRepository<PaymentSettings, int>().GetAllAsync();
            var setting = settings.FirstOrDefault(s => s.Method == paymentMethod);

            if (setting == null)
                throw new PaymentNotFoundException(nameof(PaymentSettings), method);

            setting.PhoneNumber = phoneNumber;
            _unitOfWork.GetRepository<PaymentSettings, int>().Update(setting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaymentProofResultDto?> GetPaymentProofAsync(int orderId, bool isAdmin)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId.ToString());

            if (!order.PaymentProofId.HasValue)
                return null;

            var proof = await _unitOfWork.GetRepository<OrderPaymentProof, int>().GetAsync(order.PaymentProofId.Value);
            if (proof == null)
                return null;

            var result = _mapper.Map<PaymentProofResultDto>(proof);
            
            // Hide file URL for non-admin
            if (!isAdmin)
            {
                result.FileUrl = "[REDACTED - Admin Only]";
            }

            return result;
        }

        public async Task DeletePaymentProofAsync(int proofId, string adminId)
        {
            var proof = await _unitOfWork.GetRepository<OrderPaymentProof, int>().GetAsync(proofId);
            if (proof == null)
                throw new PaymentNotFoundException(nameof(OrderPaymentProof), proofId.ToString());

            // Delete file from R2
            await _fileStorage.DeleteAsync(proof.FileUrl);

            // Remove reference from order
            var order = await _unitOfWork.GetRepository<Order, int>().GetAsync(proof.OrderId);
            if (order != null && order.PaymentProofId == proofId)
            {
                order.PaymentProofId = null;
                order.PaymentStatus = PaymentStatus.Pending;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Order, int>().Update(order);
            }

            // Delete proof record
            _unitOfWork.GetRepository<OrderPaymentProof, int>().Delete(proof);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
