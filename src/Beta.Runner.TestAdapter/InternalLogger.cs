using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Runner.TestAdapter;

public class InternalLogger(IMessageLogger? logger, Stopwatch stopwatch)
{
    public void Log(string format, params object?[] args)
    {
        Log(TestMessageLevel.Informational, format, args);
    }

    public void Log(TestMessageLevel level, string format, params object?[] args)
    {
        SendMessage(level, string.Format(format, args));
    }

    private void SendMessage(TestMessageLevel level, string message)
    {
        SendMessage(level, null, message);
    }

    private void SendMessage(TestMessageLevel level, string? assemblyName, string message)
    {
        var assemblyPath = assemblyName is not null ? string.Empty : Path.GetFileNameWithoutExtension(assemblyName);
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

        logger?.SendMessage(level, $"[Beta {elapsed}] {assemblyPath}:{message}");
    }
}