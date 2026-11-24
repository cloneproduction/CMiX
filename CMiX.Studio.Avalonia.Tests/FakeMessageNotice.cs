using CMiX.Studio.Avalonia.Notices;

namespace CMiX.Studio.Avalonia.Tests
{
    // Keeps the error messages and their titles without a window.
    public sealed class FakeMessageNotice : IMessageNotice
    {
        public List<(string Text, string Title)> Messages { get; } = new();

        public Task ShowErrorAsync(string text, string title)
        {
            Messages.Add((text, title));
            return Task.CompletedTask;
        }
    }
}
