namespace Beta.Engine.Api;

[Serializable]
public class TestPackage
{
    private static int _nextId;

    /// <summary>
    ///     Construct a named TestPackage, specifying a file path for
    ///     the assembly or project to be used.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public TestPackage(string? filePath)
    {
        Id = GetNextId();

        if (filePath == null)
        {
            return;
        }

        FullName = Path.GetFullPath(filePath);
        Settings = new Dictionary<string, object>();
        SubPackages = [];
    }

    /// <summary>
    ///     Construct an anonymous TestPackage that wraps test files.
    /// </summary>
    /// <param name="testFiles"></param>
    public TestPackage(IList<string> testFiles)
    {
        Id = GetNextId();
        SubPackages = [];
        Settings = new Dictionary<string, object>();

        foreach (var testFile in testFiles)
        {
            SubPackages.Add(new TestPackage(testFile));
        }
    }

    /// <summary>
    ///     Every test package gets a unique ID used to prefix test IDs within that package.
    /// </summary>
    /// <remarks>
    ///     The generated ID is only unique for packages created within the same application domain.
    ///     For that reason, NUnit pre-creates all test packages that will be needed.
    /// </remarks>
    public string Id { get; private set; }

    /// <summary>
    ///     Gets the name of the package
    /// </summary>
    public string? Name => Path.GetFileName(FullName);

    /// <summary>
    ///     Gets the path to the file containing tests. It may be
    ///     an assembly or a recognized project type.
    /// </summary>
    public string? FullName { get; private set; }

    /// <summary>
    ///     Gets the list of SubPackages contained in this package
    /// </summary>
    public List<TestPackage> SubPackages { get; private set; } = [];

    /// <summary>
    ///     Gets the settings dictionary for this package.
    /// </summary>
    public Dictionary<string, object> Settings { get; private set; } = new();

    private string GetNextId()
    {
        return (_nextId++).ToString();
    }

    /// <summary>
    ///     Add a subproject to the package.
    /// </summary>
    /// <param name="subPackage">The subpackage to be added</param>
    public void AddSubPackage(TestPackage subPackage)
    {
        SubPackages.Add(subPackage);

        foreach (var key in Settings.Keys)
        {
            subPackage.Settings[key] = Settings[key];
        }
    }

    /// <summary>
    ///     Add a setting to a package and all of its subpackages.
    /// </summary>
    /// <param name="name">The name of the setting</param>
    /// <param name="value">The value of the setting</param>
    /// <remarks>
    ///     Once a package is created, subpackages may have been created
    ///     as well. If you add a setting directly to the Settings dictionary
    ///     of the package, the subpackages are not updated. This method is
    ///     used when the settings are intended to be reflected to all the
    ///     subpackages under the package.
    /// </remarks>
    public void AddSetting(string name, object value)
    {
        Settings[name] = value;

        foreach (var subPackage in SubPackages)
        {
            subPackage.AddSetting(name, value);
        }
    }

    /// <summary>
    ///     Return the value of a setting or a default.
    /// </summary>
    /// <param name="name">The name of the setting</param>
    /// <param name="defaultSetting">The default value</param>
    /// <returns></returns>
    public T GetSetting<T>(string name, T defaultSetting)
    {
        return Settings.TryGetValue(name, out var setting)
            ? (T)setting
            : defaultSetting;
    }
}
