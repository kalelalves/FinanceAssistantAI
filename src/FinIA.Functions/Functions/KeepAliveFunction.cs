using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FinIA.Functions.Functions;

public sealed class KeepAliveFunction(ILogger<KeepAliveFunction> logger)
{
    [Function("KeepAlive")]
    public void Run([TimerTrigger("0 0 9 */3 * *")] TimerInfo timerInfo)
    {
        _ = timerInfo;
        logger.LogInformation("FinIA keep-alive executed at {ExecutedAtUtc}", DateTimeOffset.UtcNow);
    }
}
