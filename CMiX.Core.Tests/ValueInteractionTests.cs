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
            provider.GetRequiredService<ControlRegistry>().ActivateAll();
            return control;
        }

        [Fact]
        public void Scope_OfManyWrites_CollapsesThemIntoOneUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var control = CreateActiveDouble(provider);

            using (interaction.BeginScope())
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
        public void Scope_OfManyWrites_ThrottlesSendsAndSendsTheFinalValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var recorder = new RecordingMessageSender();
            messenger.Register(recorder);
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var control = CreateActiveDouble(provider);

            using (interaction.BeginScope())
            {
                for (int i = 1; i <= 500; i++)
                    control.Value = i;
            }

            messenger.Unregister(recorder);
            var values = recorder.Sent.Select(message => ((GenericValueModel<double>)((MessageValueChanged)message).Value).Value).ToList();

            Assert.NotEmpty(values);
            Assert.True(values.Count < 500, $"expected throttled sends, got {values.Count}");
            Assert.Equal(500d, values[^1]);
        }

        [Fact]
        public void Value_WithoutAScope_SendsEveryWrite()
        {
            var sync = new FakeValueSync();
            var control = sync.CreateActive<double>();

            for (int i = 1; i <= 20; i++)
                control.Value = i;

            Assert.Equal(Enumerable.Repeat(new[] { "Changing", "Changed" }, 20).SelectMany(pair => pair), sync.Calls);
        }

        [Fact]
        public void Dispose_OfAStaleHandle_LeavesTheNewerScopeAlone()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var control = CreateActiveDouble(provider);

            var abandoned = interaction.BeginScope();
            abandoned.Dispose();

            var current = interaction.BeginScope();
            control.Value = 1;
            control.Value = 2;

            // Both stand for a control that never opened this scope: one whose own gesture is
            // over and one that never opened a scope at all. Neither may flush the open gesture.
            abandoned.Dispose();
            default(ValueInteractionScope).Dispose();
            Assert.True(interaction.IsActive);

            control.Value = 3;
            current.Dispose();

            Assert.False(interaction.IsActive);
            Assert.Equal(3d, control.Value);

            undoManager.Undo();

            Assert.Equal(0d, control.Value);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void End_WithoutAHandle_CollapsesEveryWriteIntoOneUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var control = CreateActiveDouble(provider);

            interaction.BeginScope();
            control.Value = 1;
            control.Value = 2;
            interaction.End();

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
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var control = CreateActiveDouble(provider);

            var scope = interaction.BeginScope();
            control.Value = 7;
            interaction.End();

            Assert.False(interaction.IsActive);
            Assert.True(undoManager.CanUndo);

            // The handle of a scope somebody else already ended closes nothing.
            control.Value = 8;
            scope.Dispose();

            Assert.Equal(8d, control.Value);
        }
    }
}
