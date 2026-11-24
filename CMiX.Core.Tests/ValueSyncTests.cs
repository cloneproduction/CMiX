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
            public ValueInteraction Interaction { get; init; }
            public ValueSync Sync { get; init; }
            public FakeTimeProvider Time { get; init; }
            public ControlMessenger Messenger { get; init; }
            public RecordingMessageSender Sender { get; init; }
            public UndoManager UndoManager { get; init; }
            public GenericValue<float> Value { get; init; }

            public void Change()
            {
                Sync.Changing(Value);
                Sync.Changed(Value);
            }

            public void AssertOneValueChanged()
            {
                var message = Assert.IsType<MessageValueChanged>(Assert.Single(Sender.Sent));
                Assert.Equal(Value.ID, message.ID);
            }
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
            var interaction = provider.GetRequiredService<ValueInteraction>();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            var time = new FakeTimeProvider { Now = 1000 };
            var sync = new ValueSync(undoManager, provider.GetRequiredService<MessageFactory>(), messenger, interaction, time);
            var value = new GenericValue<float>(sync, provider.GetRequiredService<ControlActivationService>());
            value.Activate();
            return new Fixture
            {
                Interaction = interaction,
                Sync = sync,
                Time = time,
                Messenger = messenger,
                Sender = sender,
                UndoManager = undoManager,
                Value = value,
            };
        }

        [Fact]
        public void Changing_ThenChanged_PushesOneValueChangedCommandAndSendsOneMessage()
        {
            var fixture = Create();

            fixture.Change();

            Assert.True(fixture.UndoManager.CanUndo);
            fixture.AssertOneValueChanged();
        }

        [Fact]
        public void Changing_ThenChangedWhileTheUndoManagerApplies_SendsButPushesNothing()
        {
            var fixture = Create();
            fixture.UndoManager.Push(new ActionUndoCommand(fixture.Change));

            fixture.UndoManager.Undo();

            Assert.False(fixture.UndoManager.CanUndo);
            Assert.True(fixture.UndoManager.CanRedo);
            fixture.AssertOneValueChanged();
        }

        [Fact]
        public void Send_OutsideADrag_SendsOneMessageAndPushesNothing()
        {
            var fixture = Create();

            fixture.Sync.Send(fixture.Value);

            Assert.False(fixture.UndoManager.CanUndo);
            fixture.AssertOneValueChanged();
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

            fixture.Interaction.BeginScope();
            for (int i = 0; i < 100; i++)
                fixture.Change();

            Assert.Single(fixture.Sender.Sent);
            Assert.False(fixture.UndoManager.CanUndo);

            fixture.Time.Now += 1000;
            fixture.Change();

            Assert.Equal(2, fixture.Sender.Sent.Count);
        }

        [Fact]
        public void EndInteraction_WithAPendingSend_SendsOnceThenRecordsOnce()
        {
            var fixture = Create();

            using (fixture.Interaction.BeginScope())
            {
                fixture.Change();
                fixture.Change();
            }

            Assert.Equal(2, fixture.Sender.Sent.Count);
            Assert.True(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void EndInteraction_WithNothingPending_RecordsAndSendsNothingMore()
        {
            var fixture = Create();

            using (fixture.Interaction.BeginScope())
                fixture.Change();

            fixture.AssertOneValueChanged();
            Assert.True(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void EndInteraction_OfAMomentaryValue_SendsThePendingValueAndRecordsNothing()
        {
            var fixture = Create();

            using (fixture.Interaction.BeginScope())
            {
                fixture.Sync.Send(fixture.Value);
                fixture.Sync.Send(fixture.Value);
            }

            Assert.Equal(2, fixture.Sender.Sent.Count);
            Assert.False(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void Send_DuringADrag_IsThrottledAndRecordsNothing()
        {
            var fixture = Create();

            using (fixture.Interaction.BeginScope())
            {
                for (int i = 0; i < 50; i++)
                    fixture.Sync.Send(fixture.Value);

                Assert.Single(fixture.Sender.Sent);
                Assert.False(fixture.UndoManager.CanUndo);

                fixture.Time.Now += 1000;
                fixture.Sync.Send(fixture.Value);

                Assert.Equal(2, fixture.Sender.Sent.Count);
            }

            Assert.Equal(2, fixture.Sender.Sent.Count);
            Assert.False(fixture.UndoManager.CanUndo);
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
