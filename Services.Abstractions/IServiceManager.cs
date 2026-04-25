using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        public IProductService ProductService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IBasketService BasketService { get; }
        public IOrderService OrderService { get; }
        public IPaymentService PaymentService { get; }
        public IWishlistService WishlistService { get; }
        public IDashboardService DashboardService { get; }
    }
}