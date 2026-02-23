# Logging

## Overview
The application uses [Serilog](https://serilog.net/) for structured logging with two sinks: console (development) and rolling file (production).

## Configuration

### Program.cs Setup
Serilog is configured as the sole logging provider via `builder.Host.UseSerilog()`. The logger is created early in `Main()` so startup errors are captured.

### Log Levels
| Source | Level |
|--------|-------|
| Application code | Information |
| Microsoft.* | Warning |
| Microsoft.EntityFrameworkCore | Warning |
| Microsoft.AspNetCore | Warning |
| System.* | Warning |

### Sinks

**Console** — Human-readable, timestamp + level + message:
```
[14:32:01 INF] Starting BitcoinClub application
[14:32:05 INF] HTTP GET / responded 200 in 12.3456ms
```

**File** — Machine-parseable, daily rolling in `logs/`:
```
2026-02-23 14:32:01.234 +02:00 [INF] BitcoinClub.Program Starting BitcoinClub application
```
- Path: `logs/bitcoinclub-YYYYMMDD.log`
- Retention: 30 days (older files auto-deleted)
- Includes source context for filtering

### HTTP Request Logging
All HTTP requests are logged via `app.UseSerilogRequestLogging()` with:
- Method, path, status code, elapsed time
- Template: `HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms`

## What Gets Logged

### Authentication
- User registration (success + email)
- Registration failures (email + error details)

### Payments
- Payment initiation (user ID, amount in sats)
- Payment verification requests
- Payment status changes (pending, paid)

### Application Lifecycle
- Application start
- Fatal errors / unexpected termination
- Graceful shutdown (via `Log.CloseAndFlushAsync()`)

## Extending

To add logging to a new controller or service:

```csharp
public class MyController : Controller
{
    private readonly ILogger<MyController> _logger;

    public MyController(ILogger<MyController> logger)
    {
        _logger = logger;
    }

    public IActionResult MyAction()
    {
        _logger.LogInformation("Something happened for {UserId}", userId);
        return View();
    }
}
```

Use structured logging (named placeholders, not string interpolation) for searchability.

## Configuration Override

Adjust levels in `appsettings.json` under `Serilog.MinimumLevel.Override` or via environment variables:
```
Serilog__MinimumLevel__Default=Debug
```
