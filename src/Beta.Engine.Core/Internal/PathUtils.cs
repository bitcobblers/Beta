﻿// ReSharper disable once CheckNamespace

using System.Text;

namespace Beta.Engine.Internal;

/// <summary>
///     Static methods for manipulating project paths, including both directories
///     and files. Some synonyms for System.Path methods are included as well.
/// </summary>
public class PathUtils
{
    public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
    public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
    public const int MAX_PATH = 256;

    protected static char DirectorySeparatorChar = Path.DirectorySeparatorChar;
    protected static char AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;

    /// <summary>
    ///     Returns a boolean indicating whether the specified path
    ///     is that of an assembly - that is a dll or exe file.
    /// </summary>
    /// <param name="path">Path to a file.</param>
    /// <returns>True if the file extension is dll or exe, otherwise false.</returns>
    public static bool IsAssemblyFileType(string path)
    {
        var extension = Path.GetExtension(path).ToLower();
        return extension is ".dll" or ".exe";
    }

    /// <summary>
    ///     Returns the relative path from a base directory to another
    ///     directory or file.
    /// </summary>
    public static string? RelativePath(string from, string to)
    {
        if (from == null)
        {
            throw new ArgumentNullException(from);
        }

        if (to == null)
        {
            throw new ArgumentNullException(to);
        }

        var toPathRoot = Path.GetPathRoot(to);
        if (string.IsNullOrEmpty(toPathRoot))
        {
            return to;
        }

        var fromPathRoot = Path.GetPathRoot(from);

        if (!PathsEqual(toPathRoot, fromPathRoot))
        {
            return null;
        }

        var fromNoRoot = from.Substring(fromPathRoot.Length);
        var toNoRoot = to.Substring(toPathRoot.Length);

        var _from = SplitPath(fromNoRoot);
        var _to = SplitPath(toNoRoot);

        var sb = new StringBuilder(Math.Max(from.Length, to.Length));

        int lastCommon, min = Math.Min(_from.Length, _to.Length);
        for (lastCommon = 0; lastCommon < min; ++lastCommon)
        {
            if (!PathsEqual(_from[lastCommon], _to[lastCommon]))
            {
                break;
            }
        }

        if (lastCommon < _from.Length)
        {
            sb.Append("..");
        }

        for (var i = lastCommon + 1; i < _from.Length; ++i)
        {
            sb.Append(DirectorySeparatorChar).Append("..");
        }

        if (sb.Length > 0)
        {
            sb.Append(DirectorySeparatorChar);
        }

        if (lastCommon < _to.Length)
        {
            sb.Append(_to[lastCommon]);
        }

        for (var i = lastCommon + 1; i < _to.Length; ++i)
        {
            sb.Append(DirectorySeparatorChar).Append(_to[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Return the canonical form of a path.
    /// </summary>
    public static string Canonicalize(string path)
    {
        var parts = new List<string>(
            path.Split(DirectorySeparatorChar, AltDirectorySeparatorChar));

        for (var index = 0; index < parts.Count;)
        {
            var part = parts[index];

            switch (part)
            {
                case ".":
                    parts.RemoveAt(index);
                    break;

                case "..":
                    parts.RemoveAt(index);
                    if (index > 0)
                    {
                        parts.RemoveAt(--index);
                    }

                    break;
                default:
                    index++;
                    break;
            }
        }

        // Trailing separator removal
        if (parts.Count > 1 && path.Length > 1 && parts[parts.Count - 1] == "")
        {
            parts.RemoveAt(parts.Count - 1);
        }

        return string.Join(DirectorySeparatorChar.ToString(), parts.ToArray());
    }

    /// <summary>
    ///     True if the two paths are the same or if the second is
    ///     directly or indirectly under the first. Note that paths
    ///     using different network shares or drive letters are
    ///     considered unrelated, even if they end up referencing
    ///     the same subtrees in the file system.
    /// </summary>
    public static bool SamePathOrUnder(string path1, string path2)
    {
        path1 = Canonicalize(path1);
        path2 = Canonicalize(path2);

        var length1 = path1.Length;
        var length2 = path2.Length;

        // if path1 is longer, then path2 can't be under it
        if (length1 > length2)
        {
            return false;
        }

        // if lengths are the same, check for equality
        if (length1 == length2)
        {
            return string.Compare(path1, path2, IsWindows()) == 0;
        }

        // path 2 is longer than path 1: see if initial parts match
        if (string.Compare(path1, path2.Substring(0, length1), IsWindows()) != 0)
        {
            return false;
        }

        // must match through or up to a directory separator boundary
        return path2[length1 - 1] == DirectorySeparatorChar ||
               path2[length1] == DirectorySeparatorChar;
    }

    /// <summary>
    ///     Combines all the arguments into a single path
    /// </summary>
    public static string Combine(string path1, params string[] morePaths)
    {
        return morePaths.Aggregate(path1, Path.Combine);
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified file path is absolute or not on Windows operating systems.
    /// </summary>
    /// <param name="path">Path to check</param>
    /// <returns><see langword="true" /> if <paramref name="path" /> is an absolute or UNC path; otherwhise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
    public static bool IsFullyQualifiedWindowsPath(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path.Length > 2)
        {
            return (IsValidDriveSpecifier(path[0]) && path[1] == ':' && IsWindowsDirectorySeparator(path[2]))
                   || (IsWindowsDirectorySeparator(path[0]) && IsWindowsDirectorySeparator(path[1]));
        }

        return false;
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified file path is absolute or not on Linux/macOS/Unix.
    /// </summary>
    /// <param name="path">Path to check</param>
    /// <returns><see langword="true" /> if <paramref name="path" /> is an absolute path; otherwhise, false.</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="path" />
    /// </exception>
    public static bool IsFullyQualifiedUnixPath(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return path.Length > 0 && path[0] == '/';
    }

    private static bool IsWindowsDirectorySeparator(char c)
    {
        return c is '\\' or '/';
    }

    private static bool IsValidDriveSpecifier(char c)
    {
        return c is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
    }

    private static bool IsWindows()
    {
        return DirectorySeparatorChar == '\\';
    }

    private static string[] SplitPath(string path)
    {
        char[] separators = [DirectorySeparatorChar, AltDirectorySeparatorChar];
        var trialSplit = path.Split(separators);
        var emptyEntries = trialSplit.Count(piece => piece == string.Empty);

        if (emptyEntries == 0)
        {
            return trialSplit;
        }

        var finalSplit = new string[trialSplit.Length - emptyEntries];
        var index = 0;

        foreach (var piece in trialSplit)
        {
            if (piece != string.Empty)
            {
                finalSplit[index++] = piece;
            }
        }

        return finalSplit;
    }

    private static bool PathsEqual(string path1, string path2)
    {
        return IsWindows() ? path1.ToLower().Equals(path2.ToLower()) : path1.Equals(path2);
    }
}
