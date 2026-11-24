// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ValueSyncTests
    {
        private sealed class Fixture
        {
            public IServiceProvider Provider { get; init; }
            public UndoManager UndoManager { get; init; }
            public ControlMessenger Messenger { get; init; }
            public RecordingMessageSender Sender { get; init; }
            public FakeTimeProvider Time { get; init; }
            public GenericValue<float> Value { get; init; }
            public ValueSync Sync { get; init; }
        }

        private sealed class ActionUndoCommand : IUndoCommand
        {
            private readonly Action _undo;
            public ActionUndoCommand(Action undo) => _undo = undo;
            public void Execute() { }
            public void Undo() => _undo();
        }

        private static Fixture Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            var time = new FakeTimeProvider { Now = 1000 };
            var sync = new ValueSync(undoManager,
                                     provider.GetRequiredService<MessageFactory>(),
                                     messenger,
                                     provider.GetRequiredService<ValueInteraction>(),
                                     time);
            var value = new GenericValue<float>(sync, provider.GetRequiredService<ControlActivationService>());
            value.Activate();
            return new Fixture
            {
                Provider = provider,
                UndoManager = undoManager,
                Messenger = messenger,
                Sender = sender,
                Time = time,
                Value = value,
                Sync = sync,
            };
        }

        [Fact]
        public void Changing_ThenChanged_PushesOneValueChangedCommandAndSendsOneMessage()
        {
            var fixture = Create();

            fixture.Sync.Changing(fixture.Value);
            fixture.Sync.Changed(fixture.Value);

            Assert.True(fixture.UndoManager.CanUndo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Changing_ThenChangedWhileTheUndoManagerApplies_SendsButPushesNothing()
        {
            var fixture = Create();
            fixture.UndoManager.Push(new ActionUndoCommand(() =>
            {
                fixture.Sync.Changing(fixture.Value);
                fixture.Sync.Changed(fixture.Value);
            }));

            fixture.UndoManager.Undo();

            Assert.False(fixture.UndoManager.CanUndo);
            Assert.True(fixture.UndoManager.CanRedo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Send_OutsideADrag_SendsOneMessageAndPushesNothing()
        {
            var fixture = Create();

            fixture.Sync.Send(fixture.Value);

            Assert.False(fixture.UndoManager.CanUndo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Send_WhenTheMessengerIsBlocked_SendsNothing()
        {
            var fixture = Create();
            fixture.Messenger.IsSendingBlocked = true;

            fixture.Sync.Send(fixture.Value);

            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void Changed_DuringAScope_SendsAtMostOncePerIntervalAndRecordsNothingUntilTheEnd()
        {
            var fixture = Create();
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();

            try
            {
                interaction.BeginScope();
                for (int i = 0; i < 100; i++)
                {
                    fixture.Sync.Changing(fixture.Value);
                    fixture.Sync.Changed(fixture.Value);
                }

                Assert.Single(fixture.Sender.Sent);
                Assert.False(fixture.UndoManager.CanUndo);

                fixture.Time.Now += 1000;
                fixture.Sync.Changing(fixture.Value);
                fixture.Sync.Changed(fixture.Value);

                Assert.Equal(2, fixture.Sender.Sent.Count);
            }
            finally
            {
                interaction.End();
            }
        }

        [Fact]
        public void EndInteraction_WithAPendingSend_SendsOnceThenRecordsOnce()
        {
            var fixture = Create();
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();

            try
            {
                using (interaction.BeginScope())
                {
                    fixture.Sync.Changing(fixture.Value);
                    fixture.Sync.Changed(fixture.Value);
                    fixture.Sync.Changing(fixture.Value);
                    fixture.Sync.Changed(fixture.Value);
                }

                Assert.Equal(2, fixture.Sender.Sent.Count);
                Assert.True(fixture.UndoManager.CanUndo);
            }
            finally
            {
                interaction.End();
            }
        }

        [Fact]
        public void EndInteraction_WithNothingPending_RecordsAndSendsNothingMore()
        {
            var fixture = Create();
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();

            try
            {
                using (interaction.BeginScope())
                {
                    fixture.Sync.Changing(fixture.Value);
                    fixture.Sync.Changed(fixture.Value);
                }

                Assert.Single(fixture.Sender.Sent);
                Assert.True(fixture.UndoManager.CanUndo);
            }
            finally
            {
                interaction.End();
            }
        }

        [Fact]
        public void EndInteraction_OfAMomentaryValue_SendsThePendingValueAndRecordsNothing()
        {
            var fixture = Create();
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();

            try
            {
                using (interaction.BeginScope())
                {
                    fixture.Sync.Send(fixture.Value);
                    fixture.Sync.Send(fixture.Value);
                }

                Assert.Equal(2, fixture.Sender.Sent.Count);
                Assert.False(fixture.UndoManager.CanUndo);
            }
            finally
            {
                interaction.End();
            }
        }

        [Fact]
        public void ValueSyncFactory_Called_GivesANewSyncEachTime()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ValueSyncFactory>();

            var first = factory();
            var second = factory();

            Assert.NotSame(first, second);
        }
    }
}
