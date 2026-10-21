using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SuppressSendingTests
    {
        private static GenericValue<double> CreateActiveDouble(IServiceProvider provider)
        {
            var control = provider.GetRequiredService<GenericValue<double>>();
            provider.GetRequiredService<ControlActivationService>().ActivateAll();
            return control;
        }

        [Fact]
        public void SuppressSending_Write_SendsNothingAndLeavesNoUndoStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var recorder = new CountingSender();
            messenger.Register(recorder);
            var control = CreateActiveDouble(provider);

            using (control.SuppressSending())
                control.Value = 5;

            messenger.Unregister(recorder);

            Assert.Equal(5d, control.Value);
            Assert.Equal(0, recorder.Count);
            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void SuppressSending_WriteAfterScope_SendsAgain()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var recorder = new CountingSender();
            messenger.Register(recorder);
            var control = CreateActiveDouble(provider);

            using (control.SuppressSending())
                control.Value = 5;
            control.Value = 6;

            messenger.Unregister(recorder);

            Assert.Equal(1, recorder.Count);
        }

        [Fact]
        public void SuppressSending_WriteAfterInnerScopeEnds_StaysSuppressedUntilTheOuterScopeEnds()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var recorder = new CountingSender();
            messenger.Register(recorder);
            var control = CreateActiveDouble(provider);

            using (control.SuppressSending())
            {
                using (control.SuppressSending())
                    control.Value = 1;
                control.Value = 2;
            }

            messenger.Unregister(recorder);

            Assert.False(control.IsSendingSuppressed);
            Assert.Equal(0, recorder.Count);
        }

        [Fact]
        public void SuppressSending_OnAControlThatIsNotActive_DoesNotActivateIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var control = provider.GetRequiredService<GenericValue<double>>();

            using (control.SuppressSending())
                control.Value = 3;

            Assert.False(control.IsActive);
        }

        private sealed class CountingSender : IMessageSender
        {
            public int Count { get; private set; }

            public void SendMessage(IMessage message)
            {
                if (message is MessageValueChanged) Count++;
            }
        }
    }
}
