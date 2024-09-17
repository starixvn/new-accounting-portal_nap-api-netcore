using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Nap.Auth.Service;
using ONEbook.OTP.Services;

namespace NAP.AuthWebApi;

using Fanex.Caching.Redis;
using Fanex.Logging.Sentry.RavenClient;
using Nap.Auth.DataCachingPush;
using SharpRaven;

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
        services.AddTransient<OtpService, OtpServiceImpl>();

        return services;
    }

    public static IServiceCollection AddFanexServices(this IServiceCollection services, IConfiguration configuration)
    {
        LogManager
        .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
        .Use(new List<ILogEngine>
        {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
        });

        ExceptionHandler
            .UseConfig(new LoggingConfiguration())
            .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

        return services;
    }

    public static void AddFanexCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));
    }

    public static IServiceCollection AddNapApiCachingServices(this IServiceCollection services)
    {
        services.AddSingleton<DataBatchLoadingService, DataBatchLoadingServiceImp>();
        services.AddSingleton<DataCachingService, DataCachingServiceImp>();

        return services;
    }
}
