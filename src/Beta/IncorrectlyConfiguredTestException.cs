using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Beta;

[ExcludeFromCodeCoverage, Serializable]
public class IncorrectlyConfiguredTestException : Exception
{
    public IncorrectlyConfiguredTestException(string name, string[] errors)
        : base($"The test '{name}' contains {errors.Length} errors: {string.Join(" ", errors)}")
    {
        Name = name;
        Errors = errors;
    }

    protected IncorrectlyConfiguredTestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Name = info.GetString(nameof(Name))!;
        Errors = (string[])info.GetValue(nameof(Errors), typeof(string[]))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(Errors), Errors);
    }

    public string Name { get; private set; }
    public string[] Errors { get; private set; }
}