using Microsoft.Extensions.DependencyInjection;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;

namespace Puzzle.Masroofi.ServiceInterface
{
    public static class ServiceCollectionSetup
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRoleActionsRepository, RoleActionsRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IGovernorateRepository, GovernorateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IParentRepository, ParentRepository>();
            services.AddScoped<IParentLoginHistoryRepository, ParentLoginHistoryRepository>();
            services.AddScoped<ISonRepository, SonRepository>();
            services.AddScoped<IATMCardRepository, ATMCardRepository>();
            services.AddScoped<IATMCardTypeRepository, ATMCardTypeRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<IPOSMachineRepository, POSMachineRepository>();
            services.AddScoped<IPOSMachineLoginHistoryRepository, POSMachineLoginHistoryRepository>();
            services.AddScoped<ICommissionRepository, CommissionRepository>();
            services.AddScoped<IParentWalletTransactionRepository, ParentWalletTransactionRepository>();
            services.AddScoped<IATMCardTransactionRepository, ATMCardTransactionRepository>();
            services.AddScoped<IPOSMachineTransactionRepository, POSMachineTransactionRepository>();
            services.AddScoped<IParentMobileRegistrationRepository, ParentMobileRegistrationRepository>();
            services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IGovernorateService, GovernorateService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<ISonService, SonService>();
            services.AddScoped<IATMCardService, ATMCardService>();
            services.AddScoped<IATMCardTypeService, ATMCardTypeService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IPOSMachineService, POSMachineService>();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<IParentWalletTransactionService, ParentWalletTransactionService>();
            services.AddScoped<IATMCardTransactionService, ATMCardTransactionService>();
            services.AddScoped<IPOSMachineTransactionService, POSMachineTransactionService>();
            services.AddScoped<IParentMobileRegistrationService, ParentMobileRegistrationService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();

            services.AddScoped<IS3Service, S3Service>();
            services.AddScoped<ISmsService, SmsService>();
        }
    }
}
