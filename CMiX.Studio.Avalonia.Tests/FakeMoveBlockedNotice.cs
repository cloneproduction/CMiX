using CMiX.Studio.Avalonia.Notices;

namespace CMiX.Studio.Avalonia.Tests
{
    // Keeps the messages of the blocked moves without a window.
    public sealed class FakeMoveBlockedNotice : IMoveBlockedNotice
    {
        public List<string> Messages { get; } = new();

        public Task ShowAsync(string message)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
