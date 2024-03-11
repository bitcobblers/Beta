using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter.Tests.Fakes;

public class MessageLoggerStub : IMessageLogger
{
    private readonly List<Tuple<TestMessageLevel, string>> _messages = [];

    public TestMessageLevel LatestTestMessageLevel => _messages.Last().Item1;
    public string LatestMessage => _messages.Last().Item2;

    public int Count => _messages.Count;

    public IEnumerable<Tuple<TestMessageLevel, string>> Messages => _messages;

    public IEnumerable<Tuple<TestMessageLevel, string>> WarningMessages =>
        _messages.Where(o => o.Item1 == TestMessageLevel.Warning);

    public IEnumerable<Tuple<TestMessageLevel, string>> ErrorMessages =>
        _messages.Where(o => o.Item1 == TestMessageLevel.Error);

    public void SendMessage(TestMessageLevel testMessageLevel, string message)
    {
        _messages.Add(new Tuple<TestMessageLevel, string>(testMessageLevel, message));
    }
}
