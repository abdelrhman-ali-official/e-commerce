using Microsoft.AspNetCore.Http;
using Shared.PaymentModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IPaymentService
    {
        Task<PaymentProofResultDto> UploadPaymentProofAsync(int orderId, string? userId, string? basketId, IFormFile file, string payerPhone);
        Task ApprovePaymentAsync(int orderId, string adminId);
        Task RejectPaymentAsync(int orderId, string adminId, string? rejectionReason);
        Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync();
        Task UpdatePaymentSettingAsync(string method, string phoneNumber);
        Task<PaymentProofResultDto?> GetPaymentProofAsync(int orderId, bool isAdmin);
        Task DeletePaymentProofAsync(int proofId, string adminId);
    }
}
