using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter.Tests.Fakes;

public class MessageLoggerStub : IMessageLogger
{
    private readonly List<Tuple<TestMessageLevel, string>> messages = [];

    public TestMessageLevel LatestTestMessageLevel => messages.Last().Item1;
    public string LatestMessage => messages.Last().Item2;

    public int Count => messages.Count;

    public IEnumerable<Tuple<TestMessageLevel, string>> Messages => messages;

    public IEnumerable<Tuple<TestMessageLevel, string>> WarningMessages =>
        messages.Where(o => o.Item1 == TestMessageLevel.Warning);

    public IEnumerable<Tuple<TestMessageLevel, string>> ErrorMessages =>
        messages.Where(o => o.Item1 == TestMessageLevel.Error);

    public void SendMessage(TestMessageLevel testMessageLevel, string message)
    {
        messages.Add(new Tuple<TestMessageLevel, string>(testMessageLevel, message));
    }
}
