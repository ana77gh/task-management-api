using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;

public class LoggingService<T> : ILoggingService<T>
{
    private readonly ILogger<T> _logger;

    public LoggingService(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);

    public void LogError(string message, params object[] args)
        => _logger.LogError(message, args);
}
