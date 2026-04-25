using Domain.Contracts;
//using Domain.Contracts.NewModule;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Abstractions;
//using Services.Services;
using Shared.SecurityModels;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AutoMapper;
using Domain.Contracts;
using Persistence.Data;
using Core.Services;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IBasketService> _basketService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPaymentService> _paymentService;
        private readonly Lazy<IWishlistService> _wishlistService;
        private readonly Lazy<IDashboardService> _dashboardService;
        private readonly AutoMapper.IMapper _mapper;
    

        //private readonly Lazy<IClinicSearchService> _clinicSearchService;

        public ServiceManager(
            IUnitOFWork unitOfWork,
            AutoMapper.IMapper mapper,
            UserManager<User> userManager,
            IOptions<JwtOptions> jwtOptions,
            IOptions<DomainSettings> domainSettings,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));

            // Initialize authenticationService
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                userManager,
                jwtOptions,
                domainSettings,
                mapper,
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>()
            ));

            // Initialize basket and order services
            _basketService = new Lazy<IBasketService>(() => new BasketService(unitOfWork, mapper));
            _orderService = new Lazy<IOrderService>(() => new OrderService(
                unitOfWork, 
                mapper, 
                serviceProvider.GetRequiredService<IFileStorageService>()));
            
            // Initialize payment service
            _paymentService = new Lazy<IPaymentService>(() => new PaymentService(
                unitOfWork,
                mapper,
                serviceProvider.GetRequiredService<IFileStorageService>()
            ));
            
            // Initialize wishlist service
            _wishlistService = new Lazy<IWishlistService>(() => new WishlistService(unitOfWork, mapper));
            
            // Initialize dashboard service
            _dashboardService = new Lazy<IDashboardService>(() => new DashboardService(unitOfWork, mapper, userManager));
/*
            _paymentService = new Lazy<IPaymentService>(() => new PaymentService(basketRepository, unitOfWork, mapper, configuration));
            _petService = new Lazy<IPetService>(() => new PetService(
                serviceProvider.GetRequiredService<IPetRepository>(),
                mapper,
                LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PetService>()));

            _clinicService = new Lazy<IClinicService>(() => new ClinicService(unitOfWork, mapper, userManager));
            _doctorScheduleService = new Lazy<IDoctorScheduleService>(() => new DoctorScheduleService(unitOfWork, mapper));

            // Use the AppointmentService from Core/Services
            _appointmentService = new Lazy<IAppointmentService>(() => new AppointmentService(unitOfWork, mapper));

            // Use the MedicalRecordService from Core/Services
            _medicalRecordService = new Lazy<IMedicalRecordService>(() => new MedicalRecordService(
                unitOfWork,
                mapper,
                _appointmentService.Value));
*/


         
        }

        public IProductService ProductService => _productService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IBasketService BasketService => _basketService.Value;
        public IOrderService OrderService => _orderService.Value;
        public IPaymentService PaymentService => _paymentService.Value;
        public IWishlistService WishlistService => _wishlistService.Value;
        public IDashboardService DashboardService => _dashboardService.Value;
            }
}