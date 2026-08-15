using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class UndoManagerTests
    {
        private static GenericValue<double> CreateActiveDouble(IServiceProvider provider)
        {
            var control = provider.GetRequiredService<GenericValue<double>>();
            provider.GetRequiredService<ControlActivationService>().ActivateAll();
            return control;
        }

        [Fact]
        public void Push_Then_Undo_RestoresPriorState()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            control.Value = 42d;

            Assert.True(undoManager.CanUndo);
            undoManager.Undo();

            Assert.Equal(0d, control.Value);
        }

        [Fact]
        public void Undo_Then_Redo_ReappliesChange()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            control.Value = 42d;
            undoManager.Undo();
            undoManager.Redo();

            Assert.Equal(42d, control.Value);
            Assert.False(undoManager.CanRedo);
        }

        [Fact]
        public void MergeWindow_TwoPushesOnSameControl_CollapseToOneUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            control.Value = 1d;
            control.Value = 2d;

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void Regression_MergedPushAfterUndo_ClearsRedoStack()
        {
            // Reproduces the bug fixed in commit 56258859: without clearing the redo stack
            // inside the merge branch, undoing B then merging a new change into A left the
            // undone B sitting redoable on top of a stack whose bottom entry had just moved.
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var controlA = CreateActiveDouble(provider);
            var controlB = CreateActiveDouble(provider);

            controlA.Value = 1d;   // push A: 0 -> 1
            controlB.Value = 1d;   // push B: 0 -> 1, different ControlID so no merge

            undoManager.Undo();    // undoes B, redo stack now holds B

            controlA.Value = 2d;   // merges into the A entry still on top of the undo stack

            Assert.False(undoManager.CanRedo);

            undoManager.Redo();    // must be a no op

            Assert.Equal(0d, controlB.Value);
        }

        [Fact]
        public void TrimStack_KeepsOnlyMaxStepsAndDropsOldestEntriesFirst()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();

            const int maxSteps = 128;
            const int pushedCount = maxSteps + 2;
            var executed = new List<int>();

            for (int i = 0; i < pushedCount; i++)
                undoManager.Push(new RecordingUndoCommand(i, executed));

            while (undoManager.CanUndo)
                undoManager.Undo();

            // Only the last maxSteps pushes (ids 2..129) should have survived the trim, most
            // recently pushed first since Undo pops the stack from the top.
            var expected = Enumerable.Range(pushedCount - maxSteps, maxSteps).Reverse().ToList();
            Assert.Equal(expected, executed);
        }

        [Fact]
        public void Undo_ThrowingCommand_LeavesIsApplyingFalseAndKeepsRecording()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();

            undoManager.Push(new ThrowingUndoCommand());

            Assert.Throws<InvalidOperationException>(() => undoManager.Undo());
            Assert.False(undoManager.IsApplying);

            var control = CreateActiveDouble(provider);
            control.Value = 42d;

            Assert.True(undoManager.CanUndo);
        }

        [Fact]
        public void EndCapture_Nested_OnlyFlushesAtTheOutermostCall()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(1, executed));

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(2, executed));
            undoManager.EndCapture();

            // The inner EndCapture must not have flushed anything yet, since the outer
            // capture opened by the first BeginCapture is still open.
            Assert.False(undoManager.CanUndo);

            undoManager.Push(new RecordingUndoCommand(3, executed));
            undoManager.EndCapture();

            Assert.True(undoManager.CanUndo);

            undoManager.Undo();

            Assert.Equal(new[] { 3, 2, 1 }, executed);
        }

        [Fact]
        public void Redo_ThrowingCommand_LeavesIsApplyingFalse()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();

            undoManager.Push(new ThrowingUndoCommand(throwOnUndo: false));
            undoManager.Undo();

            Assert.Throws<InvalidOperationException>(() => undoManager.Redo());
            Assert.False(undoManager.IsApplying);
        }

        private sealed class ThrowingUndoCommand : IUndoCommand
        {
            private readonly bool _throwOnUndo;

            public ThrowingUndoCommand(bool throwOnUndo = true)
            {
                _throwOnUndo = throwOnUndo;
            }

            public void Execute() => throw new InvalidOperationException("Execute failed");

            public void Undo()
            {
                if (_throwOnUndo) throw new InvalidOperationException("Undo failed");
            }
        }

        private sealed class RecordingUndoCommand : IUndoCommand
        {
            private readonly int _id;
            private readonly List<int> _log;

            public RecordingUndoCommand(int id, List<int> log)
            {
                _id = id;
                _log = log;
            }

            public void Execute() { }
            public void Undo() => _log.Add(_id);
        }
    }
}
