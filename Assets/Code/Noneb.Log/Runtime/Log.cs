using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Noneb.Log.Runtime;

/// <summary>
/// Note: Why do you have this?
/// ---
/// 1. You were using Unity's logger package, despite it being a preview package
/// 2. Turns out Unity rightfully stop supporting that, so you have to find alternatives
/// 3. You tried ZLogger out because it's cool and shiny
/// 4. Turns out, ZLogger, seems to be a little bit unstable(throwing exceptions) and requires configurations to not fail
/// silently
/// 5. You decided structured logging isn't worth all that effort
/// 6. Time to swap out your logger to good old Unity's Debug.Log, but for the love of god you aren't going to rework every
/// log call again ever.
/// ---
/// So here we are, every problem in the world can be solved by one more layer of abstraction, we can regret our life
/// decision later.
/// </summary>
public static class Log
{
    [HideInCallstack]
    public static void Info(string category, string message, Object? context = null)
    {
        Debug.Log($"[{category}] {message}", context);
    }

    [HideInCallstack]
    public static void Warn(string category, string message, Object? context = null)
    {
        Debug.LogWarning($"[{category}] {message}", context);
    }

    [HideInCallstack]
    public static void Error(string category, string message, Object? context = null)
    {
        Debug.LogError($"[{category}] {message}", context);
    }

    [HideInCallstack]
    public static void Exception(Exception exception, Object? context = null)
    {
        Debug.LogException(exception, context);
    }
}