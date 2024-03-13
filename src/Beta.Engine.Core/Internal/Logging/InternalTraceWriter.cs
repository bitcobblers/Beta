using System.Text;

namespace Beta.Engine.Internal;

/// <summary>
///     A trace listener that writes to a separate file per domain
///     and process using it.
/// </summary>
internal class InternalTraceWriter : TextWriter
{
    private static readonly Encoding DefaultEncoding = Encoding.UTF8;

    private readonly object _sync = new();
    private TextWriter? _writer;

    /// <summary>
    ///     Construct an InternalTraceWriter that writes to a file.
    /// </summary>
    /// <param name="logPath">Path to the file to use</param>
    internal InternalTraceWriter(string logPath)
    {
        var streamWriter =
            new StreamWriter(
                new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write),
                DefaultEncoding);

        streamWriter.AutoFlush = true;
        _writer = streamWriter;
    }

    /// <summary>
    ///     Construct an InternalTraceWriter that writes to a
    ///     TextWriter provided by the caller.
    /// </summary>
    /// <param name="writer"></param>
    public InternalTraceWriter(TextWriter writer)
    {
        _writer = writer;
    }

    /// <summary>
    ///     Returns the character encoding in which the output is written.
    /// </summary>
    /// <returns>The character encoding in which the output is written.</returns>
    public override Encoding Encoding => _writer?.Encoding ?? DefaultEncoding;

    /// <summary>
    ///     Writes a character to the text string or stream.
    /// </summary>
    /// <param name="value">The character to write to the text stream.</param>
    public override void Write(char value)
    {
        lock (_sync)
        {
            _writer?.Write(value);
        }
    }

    /// <summary>
    ///     Writes a string to the text string or stream.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public override void Write(string? value)
    {
        lock (_sync)
        {
            base.Write(value);
        }
    }

    /// <summary>
    ///     Writes a string followed by a line terminator to the text string or stream.
    /// </summary>
    /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
    public override void WriteLine(string? value)
    {
        lock (_sync)
        {
            _writer?.WriteLine(value);
        }
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the
    ///     managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     true to release both managed and unmanaged resources; false to release only unmanaged
    ///     resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        lock (_sync)
        {
            if (disposing && _writer != null)
            {
                _writer.Flush();
                _writer.Dispose();
                _writer = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///     Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
    /// </summary>
    public override void Flush()
    {
        _writer?.Flush();
    }
}
