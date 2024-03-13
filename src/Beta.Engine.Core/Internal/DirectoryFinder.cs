using Beta.Common;
using Beta.Engine.Internal.FileSystemAccess;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Internal;

/// <summary>
///     DirectoryFinder is a utility class used for extended wildcard
///     selection of directories and files. It's less than a full-fledged
///     Linux-style globbing utility and more than standard wildcard use.
/// </summary>
internal sealed class DirectoryFinder : IDirectoryFinder
{
    /// <inheritdoc />
    public IEnumerable<IDirectory> GetDirectories(IDirectory? startDirectory, string? pattern)
    {
        Guard.ArgumentNotNull(startDirectory, nameof(startDirectory));
        Guard.ArgumentNotNull(pattern, nameof(pattern));

        if (Path.DirectorySeparatorChar == '\\')
        {
            pattern = pattern!.Replace(Path.DirectorySeparatorChar, '/');
        }

        var dirList = new List<IDirectory> { startDirectory! };

        while (pattern!.Length > 0)
        {
            string range;
            var sep = pattern.IndexOf('/');

            if (sep >= 0)
            {
                range = pattern.Substring(0, sep);
                pattern = pattern.Substring(sep + 1);
            }
            else
            {
                range = pattern;
                pattern = "";
            }

            if (range is "." or "")
            {
                continue;
            }

            dirList = ExpandOneStep(dirList, range).ToList();
        }

        return dirList;
    }

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(IDirectory? startDirectory, string? pattern)
    {
        Guard.ArgumentNotNull(startDirectory, nameof(startDirectory));
        Guard.ArgumentNotNullOrEmpty(pattern, nameof(pattern));

        // If there is no directory path in pattern, delegate to DirectoryInfo
        var lastSep = pattern!.LastIndexOf('/');
        if (lastSep < 0) // Simple file name entry, no path
        {
            foreach (var file in startDirectory!.GetFiles(pattern))
            {
                yield return file;
            }

            yield break;
        }

        // Otherwise split pattern into two parts around last separator
        var pattern1 = pattern.Substring(0, lastSep);
        var pattern2 = pattern.Substring(lastSep + 1);

        foreach (var dir in GetDirectories(startDirectory, pattern1))
        {
            foreach (var file in dir.GetFiles(pattern2))
            {
                yield return file;
            }
        }
    }

    private static IEnumerable<IDirectory> ExpandOneStep(IEnumerable<IDirectory> dirList, string pattern)
    {
        foreach (var dir in dirList)
        {
            switch (pattern)
            {
                case "":
                case ".":
                    yield return dir;
                    break;
                case "..":
                    {
                        if (dir.Parent != null)
                        {
                            yield return dir.Parent;
                        }

                        break;
                    }
                case "**":
                    {
                        // ** means zero or more intervening directories, so we
                        // add the directory itself to start out.
                        yield return dir;

                        foreach (var subdir in dir.GetDirectories("*", SearchOption.AllDirectories))
                        {
                            yield return subdir;
                        }

                        break;
                    }
                default:
                    {
                        //var subDirs = dir.GetDirectories(pattern, SearchOption.TopDirectoryOnly).ToArray();

                        foreach (var subdir in dir.GetDirectories(pattern, SearchOption.TopDirectoryOnly))
                        {
                            yield return subdir;
                        }

                        break;
                    }
            }
        }
    }
}
