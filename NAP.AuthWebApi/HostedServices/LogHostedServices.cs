
namespace NAP.AuthWebApi.HostedServices;

public class LogHostedService : IHostedService, IDisposable
{
    private Timer timer = null!;

    private readonly string logFilePath;

    public LogHostedService(IWebHostEnvironment env)
    {
        logFilePath = Path.Combine(env.ContentRootPath, "site.log");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(Log, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    private void Log(object? state)
    {
        var sites = string.Join(",", LogSiteData.Sites);
        File.WriteAllText(logFilePath, sites);
    }
}
