using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Nap.Auth.Service;

namespace NAP.AuthWebApi;

using Nap.Auth.DataCachingPull;
using Nap.Auth.DataCachingPush;
using ONEbook.OTP;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IDynamicRepository, DynamicRepository>();
        services.AddTransient<Fanex.Logging.ILogger, Logger>();

        return services;
    }

    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddTransient<FunctionService, FunctionServiceImp>();
        services.AddTransient<GroupService, GroupServiceImp>();
        services.AddTransient<MenuItemService, MenuItemServiceImp>();
        services.AddTransient<SiteService, SiteServiceImp>();
        services.AddTransient<UserService, UserServiceImp>();
        services.AddTransient<OtpService, OtpServiceImp>();

        return services;
    }
    
    public static IServiceCollection AddExceptionHandler(this IServiceCollection services, IConfiguration configuration)
    {
        ExceptionHandler
            .UseConfig(new LoggingConfiguration())
            .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

        return services;
    }

    public static IServiceCollection AddNapApiCachingServices(this IServiceCollection services)
    {
        services.AddSingleton<DataBatchLoadingService, DataBatchLoadingServiceImp>();
        services.AddSingleton<CachingRefresherService, CachingRefresherServiceImp>();
        services.AddSingleton<MemoryCachingStorage, MemoryCachingStorageImp>();
        services.AddSingleton<DataCachingRealTimeJob>();
        services.AddHostedService<DataCachingRealTimeJob>();

        return services;
    }
}
