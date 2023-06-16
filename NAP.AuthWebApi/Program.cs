using System.Net.Mime;
using Fanex.Data;
using Fanex.Logging.Extensions.Database;
using Microsoft.AspNetCore.Diagnostics;
using NAP.AuthWebApi;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory()

});

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddCoreServices();
builder.Services.AddFanexServices(builder.Configuration);
builder.Services.AddInternalServices();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        if (error is ObjectDbException)
        {
            await context.Response.WriteAsync($"\nErrorMessage:{error.Message}. DATABASE INFO: \n");
            var dataDbExceptionInfo = DatabaseInfoGetters.GetDbExceptionInfo(error);
            var json = JsonConvert.SerializeObject(dataDbExceptionInfo);
            await context.Response.WriteAsync(json);
        }
    });
});
app.MapControllers();

await app.RunAsync("http://0.0.0.0:5000");