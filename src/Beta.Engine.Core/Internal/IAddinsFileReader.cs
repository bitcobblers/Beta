using Beta.Engine.Internal.FileSystemAccess;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Internal;

/// <summary>
///     A reader for NUnit addins-files.
/// </summary>
/// <remarks>
///     The format of an addins-file can be found at
///     https://docs.nunit.org/articles/nunit-engine/extensions/Installing-Extensions.html.
/// </remarks>
internal interface IAddinsFileReader
{
    /// <summary>
    ///     Reads all entries from an addins-file.
    /// </summary>
    /// <param name="file">Location of the file.</param>
    /// <returns>All entries contained in the file.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="file" /> is <see langword="null" /></exception>
    /// <exception cref="System.IO.IOException"><paramref name="file" /> cannot be found or read</exception>
    IEnumerable<string> Read(IFile? file);
}
