using System.Net.Mime;
using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.AspNetCore.Diagnostics;
using NAP.AuthWebApi;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddMemoryCache();
services.AddMaintenance(configuration);
services.AddCoreServices();
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);
services.AddExceptionHandler(configuration);
services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));
services.AddNapApiCachingServices();
services.AddInternalServices();
services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Text.Plain;
        await context.Response.WriteAsync("An exception was thrown.");

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        var error = contextFeature?.Error;
        await context.Response.WriteAsync($"\nErrorMessage:{error?.Message}.\r\n");
        if (error is ObjectDbException)
        {
            await context.Response.WriteAsync("DATABASE INFO: \n");
            var dataDbExceptionInfo = DatabaseInfoGetters.GetDbExceptionInfo(error);
            var json = JsonConvert.SerializeObject(dataDbExceptionInfo);
            await context.Response.WriteAsync(json);
        }

        await Logger.Log.ErrorAsync("An exception was thrown.", error);
        Console.WriteLine($"{error?.Message}\r\n{error?.Source}\r\n{error?.StackTrace}");
    });
});
app.MapControllers();

await app.RunAsync();