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
            provider.GetRequiredService<ControlRegistry>().ActivateAll();
            return control;
        }

        private static IServiceProvider CreateWithFakeClock(out FakeTimeProvider time)
        {
            var clock = new FakeTimeProvider { Now = 1000 };
            time = clock;
            return TestServiceProviderFactory.Create(services => services.AddSingleton<TimeProvider>(clock));
        }

        [Fact]
        public void Undo_AfterAPush_RestoresThePriorState()
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
        public void Redo_AfterAnUndo_ReappliesTheChange()
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
            var provider = CreateWithFakeClock(out _);
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            control.Value = 1d;
            control.Value = 2d;

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void MergeWindow_TwoPushesOutsideTheWindow_StayTwoSteps()
        {
            var provider = CreateWithFakeClock(out var time);
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            control.Value = 1d;
            time.Now += time.TimestampFrequency / 2;
            control.Value = 2d;

            Assert.Equal(2, undoManager.StepCount);
        }

        [Fact]
        public void Push_MergedAfterAnUndo_ClearsTheRedoStack()
        {
            // A change of A that merges into the top entry after an undo of B must clear the redo of B.
            var provider = CreateWithFakeClock(out _);
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
        public void Push_AfterAnUndoOfANewerStep_DoesNotMergeIntoAnOldStep()
        {
            var provider = CreateWithFakeClock(out var time);
            var undoManager = provider.GetRequiredService<UndoManager>();
            var controlA = CreateActiveDouble(provider);
            var controlB = CreateActiveDouble(provider);

            controlA.Value = 1d;
            time.Now += time.TimestampFrequency;
            controlB.Value = 1d;
            undoManager.Undo();
            controlA.Value = 2d;

            Assert.Equal(2, undoManager.StepCount);
        }

        [Fact]
        public void Changed_OnPushUndoRedoAndClear_FiresOncePerOperation()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var count = 0;
            undoManager.Changed += () => count++;

            undoManager.Push(new RecordingUndoCommand(1, new List<int>()));
            Assert.Equal(1, count);
            undoManager.Undo();
            Assert.Equal(2, count);
            undoManager.Redo();
            Assert.Equal(3, count);
            undoManager.Clear();
            Assert.Equal(4, count);
        }

        [Fact]
        public void Changed_OnAMergedPush_FiresOnce()
        {
            var provider = CreateWithFakeClock(out _);
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);
            control.Value = 1d;
            var count = 0;
            undoManager.Changed += () => count++;

            control.Value = 2d;

            Assert.Equal(1, count);
            Assert.Equal(1, undoManager.StepCount);
        }

        [Fact]
        public void TrimStack_OverMaxSteps_DropsTheOldestEntriesFirst()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();

            const int pushedCount = UndoManager.MaxSteps + 2;
            var executed = new List<int>();

            for (int i = 0; i < pushedCount; i++)
                undoManager.Push(new RecordingUndoCommand(i, executed));

            while (undoManager.CanUndo)
                undoManager.Undo();

            // Only the last MaxSteps pushes (ids 2..129) should have survived the trim, most
            // recently pushed first since Undo pops the stack from the top.
            var expected = Enumerable.Range(pushedCount - UndoManager.MaxSteps, UndoManager.MaxSteps).Reverse().ToList();
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

        // A throwing command inside a capture group is the gap the nesting and IsApplying tests
        // above do not pin: the group's Undo() walks the captured commands in reverse, so a command
        // that throws stops the walk before any earlier command in the group gets its own Undo, and
        // IsApplying still has to come back false since UndoManager.Undo wraps the whole composite
        // in the same try/finally as a single command.
        [Fact]
        public void EndCapture_ThrowingCommandInsideTheGroup_LeavesIsApplyingFalseAndStopsTheWalk()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(1, executed));
            undoManager.Push(new ThrowingUndoCommand());
            undoManager.EndCapture();

            Assert.True(undoManager.CanUndo);

            Assert.Throws<InvalidOperationException>(() => undoManager.Undo());

            Assert.False(undoManager.IsApplying);
            // Undo() reverses the group, so the throwing command (pushed last) runs first and the
            // exception stops the walk before command 1's Undo ever runs.
            Assert.Empty(executed);
        }

        [Fact]
        public void EndCapture_WhileTheRecordingIsSuspended_PushesNothing()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(1, executed));
            undoManager.Push(new RecordingUndoCommand(2, executed));
            using (undoManager.Suspend())
                undoManager.EndCapture();

            Assert.Equal(0, undoManager.StepCount);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void RevertCapture_TwoCapturedSteps_UndoesThemNewestFirstAndKeepsThemOffTheStack()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(1, executed));
            undoManager.Push(new RecordingUndoCommand(2, executed));
            undoManager.RevertCapture();

            Assert.Equal(new[] { 2, 1 }, executed);
            Assert.False(undoManager.CanUndo);
            Assert.False(undoManager.CanRedo);
            Assert.False(undoManager.IsApplying);
        }

        [Fact]
        public void RevertCapture_InANestedCapture_RevertsOnlyTheInnerStepsAndKeepsTheOuterOpen()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(1, executed));
            undoManager.BeginCapture();
            undoManager.Push(new RecordingUndoCommand(2, executed));
            undoManager.RevertCapture();

            Assert.Equal(new[] { 2 }, executed);
            Assert.False(undoManager.CanUndo);

            undoManager.EndCapture();
            undoManager.Undo();

            Assert.Equal(new[] { 2, 1 }, executed);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void Capture_ADisposedScope_FlushesOneStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            using (undoManager.Capture())
            {
                undoManager.Push(new RecordingUndoCommand(1, executed));
                undoManager.Push(new RecordingUndoCommand(2, executed));
                Assert.False(undoManager.CanUndo);
            }

            Assert.Equal(1, undoManager.StepCount);
            undoManager.Undo();
            Assert.Equal(new[] { 2, 1 }, executed);
        }

        [Fact]
        public void Capture_RevertThenDispose_LeavesTheStackEmpty()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            using (var scope = undoManager.Capture())
            {
                undoManager.Push(new RecordingUndoCommand(1, executed));
                scope.Revert();
            }

            Assert.Equal(new[] { 1 }, executed);
            Assert.False(undoManager.CanUndo);
            Assert.Equal(0, undoManager.StepCount);

            undoManager.Push(new RecordingUndoCommand(2, executed));
            Assert.Equal(1, undoManager.StepCount);
        }

        [Fact]
        public void Revert_OnAnOuterScopeWithAnInnerScopeOpen_RevertsBoth()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            var outer = undoManager.Capture();
            undoManager.Push(new RecordingUndoCommand(1, executed));
            var inner = undoManager.Capture();
            undoManager.Push(new RecordingUndoCommand(2, executed));
            outer.Revert();
            inner.Dispose();
            outer.Dispose();

            Assert.Equal(new[] { 2, 1 }, executed);
            Assert.Equal(0, undoManager.StepCount);

            undoManager.Push(new RecordingUndoCommand(3, executed));
            Assert.Equal(1, undoManager.StepCount);
        }

        [Fact]
        public void Capture_NestedScopes_FlushAtTheOuterDispose()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            using (undoManager.Capture())
            {
                undoManager.Push(new RecordingUndoCommand(1, executed));
                using (undoManager.Capture())
                    undoManager.Push(new RecordingUndoCommand(2, executed));
                Assert.False(undoManager.CanUndo);
                undoManager.Push(new RecordingUndoCommand(3, executed));
            }

            Assert.Equal(1, undoManager.StepCount);
            undoManager.Undo();
            Assert.Equal(new[] { 3, 2, 1 }, executed);
        }

        [Fact]
        public void Suspend_NestedScopes_ResumeOnlyAtTheOuterDispose()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            using (undoManager.Suspend())
            {
                using (undoManager.Suspend())
                {
                    undoManager.Push(new RecordingUndoCommand(1, executed));
                    Assert.Equal(0, undoManager.StepCount);
                }

                undoManager.Push(new RecordingUndoCommand(2, executed));
                Assert.Equal(0, undoManager.StepCount);
            }

            undoManager.Push(new RecordingUndoCommand(3, executed));
            Assert.Equal(1, undoManager.StepCount);
        }

        [Fact]
        public void Undo_WhileACaptureIsOpen_DoesNothing()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.Push(new RecordingUndoCommand(1, executed));
            using (undoManager.Capture())
            {
                undoManager.Push(new RecordingUndoCommand(2, executed));
                undoManager.Undo();

                Assert.Empty(executed);
                Assert.Equal(1, undoManager.StepCount);
            }

            Assert.Equal(2, undoManager.StepCount);
        }

        [Fact]
        public void Redo_WhileACaptureIsOpen_DoesNothing()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.Push(new RecordingUndoCommand(1, executed));
            undoManager.Undo();
            using (undoManager.Capture())
            {
                undoManager.Redo();

                Assert.Equal(0, undoManager.StepCount);
                Assert.Equal(1, undoManager.RedoStepCount);
            }

            Assert.Equal(0, undoManager.StepCount);
            Assert.Equal(1, undoManager.RedoStepCount);
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

        [Fact]
        public void Push_WhileApplying_RecordsNothing()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var executed = new List<int>();

            undoManager.Push(new PushingUndoCommand(undoManager, new RecordingUndoCommand(2, executed)));
            undoManager.Undo();

            Assert.False(undoManager.CanUndo);
            Assert.True(undoManager.CanRedo);
        }

        [Fact]
        public void RecordAsOneStep_TwoValueWrites_PushesOneStepOfTheControl()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var beat = provider.GetRequiredService<Animations.MasterBeat>();
            var index = beat.Index.Value;
            var beatIndex = beat.BeatIndex.Value;

            undoManager.RecordAsOneStep(beat, () =>
            {
                beat.Index.Value = 1;
                beat.BeatIndex.Value = 2;
            });

            Assert.Equal(1, undoManager.StepCount);
            undoManager.Undo();
            Assert.Equal(index, beat.Index.Value);
            Assert.Equal(beatIndex, beat.BeatIndex.Value);
        }

        private sealed class PushingUndoCommand : IUndoCommand
        {
            private readonly UndoManager _undoManager;
            private readonly IUndoCommand _pushed;

            public PushingUndoCommand(UndoManager undoManager, IUndoCommand pushed)
            {
                _undoManager = undoManager;
                _pushed = pushed;
            }

            public void Execute() { }
            public void Undo() => _undoManager.Push(_pushed);
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
