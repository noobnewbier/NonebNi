using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Noneb.Log.Runtime;

/// <summary>
/// https://github.com/Cysharp/ZLogger?tab=readme-ov-file#installation
/// Don't want to implement this everywhere so here we go, one file per assembly.
/// I can regret later
/// </summary>
public static class LogManager
{
    public static readonly ILogger Global;

    private static readonly ILoggerFactory LoggerFactory;

    static LogManager()
    {
        /*
         * Note:
         * At some point, it would be nice to:
         * 1. redirect all logs from Unity to a custom logger implementation, currently it's the other way around,
         * where ZLogger's log is piped into Unity, meaning if we only look at structured logging we are actually missing some info
         * (e.g the engine is throwing exception, which won't be caught by ZLogger in such case)
         *
         * 2. For the love of god, make a structured log analyzer within Unity, can't believe none exist atm.
         */
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create
        (logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddZLoggerUnityDebug(); // log to UnityDebug
            }
        );
        Global = LoggerFactory.CreateLogger("Logger");
        Application.exitCancellationToken.Register
        (() =>
            {
                LoggerFactory.Dispose(); // flush when application exit.
            }
        );
    }

    public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
}