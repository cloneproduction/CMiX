using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ValueInteractionTests
    {
        private static GenericValue<double> CreateActiveDouble(IServiceProvider provider)
        {
            var control = provider.GetRequiredService<GenericValue<double>>();
            provider.GetRequiredService<ControlActivationService>().ActivateAll();
            return control;
        }

        [Fact]
        public void Scope_CollapsesEveryWriteIntoASingleUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            using (ValueInteraction.BeginScope())
            {
                for (int i = 1; i <= 200; i++)
                    control.Value = i;
            }

            Assert.Equal(200d, control.Value);

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void Scope_ThrottlesSendsAndAlwaysSendsTheFinalValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var recorder = new RecordingSender();
            messenger.Register(recorder);
            var control = CreateActiveDouble(provider);

            using (ValueInteraction.BeginScope())
            {
                for (int i = 1; i <= 500; i++)
                    control.Value = i;
            }

            messenger.Unregister(recorder);

            Assert.NotEmpty(recorder.Values);
            Assert.True(recorder.Values.Count < 500, $"expected throttled sends, got {recorder.Values.Count}");
            Assert.Equal(500d, recorder.Values[^1]);
        }

        [Fact]
        public void WithoutScope_EveryWriteStillSends()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var recorder = new RecordingSender();
            messenger.Register(recorder);
            var control = CreateActiveDouble(provider);

            for (int i = 1; i <= 20; i++)
                control.Value = i;

            messenger.Unregister(recorder);

            Assert.Equal(20, recorder.Values.Count);
        }

        [Fact]
        public void StaleHandle_LeavesTheScopeOpenedAfterItAlone()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            var abandoned = ValueInteraction.BeginScope();
            abandoned.Dispose();

            var current = ValueInteraction.BeginScope();
            control.Value = 1;
            control.Value = 2;

            // Both stand for a control that never opened this scope: one whose own gesture is
            // over and one that never opened a scope at all. Neither may flush the open gesture.
            abandoned.Dispose();
            default(ValueInteractionScope).Dispose();
            Assert.True(ValueInteraction.IsActive);

            control.Value = 3;
            current.Dispose();

            Assert.False(ValueInteraction.IsActive);
            Assert.Equal(3d, control.Value);

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        // The unowned pair a host without a handle still uses, and the path Begin keeps for it.
        [Fact]
        public void UnownedScope_StillCollapsesEveryWriteIntoASingleUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            ValueInteraction.Begin();
            control.Value = 1;
            control.Value = 2;
            ValueInteraction.End();

            Assert.Equal(2d, control.Value);

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void End_WithoutAHandle_StillEndsTheOpenScope()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var control = CreateActiveDouble(provider);

            var scope = ValueInteraction.BeginScope();
            control.Value = 7;
            ValueInteraction.End();

            Assert.False(ValueInteraction.IsActive);
            Assert.True(undoManager.CanUndo);

            // The handle of a scope somebody else already ended closes nothing.
            control.Value = 8;
            scope.Dispose();

            Assert.Equal(8d, control.Value);
        }

        private sealed class RecordingSender : IMessageSender
        {
            public List<double> Values { get; } = new();

            public void SendMessage(IMessage message)
            {
                if (message is MessageValueChanged changed && changed.Value is GenericValueModel<double> model)
                    Values.Add(model.Value);
            }
        }
    }
}
