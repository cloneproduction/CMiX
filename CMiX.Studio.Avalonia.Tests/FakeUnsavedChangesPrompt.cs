using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.Tests
{
    // Answers the question about the unsaved changes without a window. By default the answer is Cancel.
    public sealed class FakeUnsavedChangesPrompt : IUnsavedChangesPrompt
    {
        public UnsavedChangesChoice Answer { get; set; } = UnsavedChangesChoice.Cancel;

        public int TimesAsked { get; private set; }

        public Task<UnsavedChangesChoice> AskAsync()
        {
            TimesAsked++;
            return Task.FromResult(Answer);
        }
    }
}
