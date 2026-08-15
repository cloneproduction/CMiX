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

            ValueInteraction.Begin();
            for (int i = 1; i <= 200; i++)
                control.Value = i;
            ValueInteraction.End();

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

            ValueInteraction.Begin();
            for (int i = 1; i <= 500; i++)
                control.Value = i;
            ValueInteraction.End();

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
