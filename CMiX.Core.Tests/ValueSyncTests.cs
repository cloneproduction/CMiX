// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
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
            public GenericValue<float> Value { get; init; }
            public IValueSync Sync { get; init; }
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
            var value = new GenericValue<float>(provider.GetRequiredService<IValueSync>(),
                                                provider.GetRequiredService<ControlActivationService>());
            return new Fixture
            {
                Provider = provider,
                UndoManager = undoManager,
                Messenger = messenger,
                Sender = sender,
                Value = value,
                Sync = provider.GetRequiredService<IValueSync>(),
            };
        }

        [Fact]
        public void Changed_WithABefore_PushesOneValueChangedCommandAndSendsOneMessage()
        {
            var fixture = Create();

            fixture.Sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

            Assert.True(fixture.UndoManager.CanUndo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Changed_WhileTheUndoManagerApplies_SendsButPushesNothing()
        {
            var fixture = Create();
            fixture.UndoManager.Push(new ActionUndoCommand(() =>
                fixture.Sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel())));

            fixture.UndoManager.Undo();

            Assert.False(fixture.UndoManager.CanUndo);
            Assert.True(fixture.UndoManager.CanRedo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Changed_WithANullBefore_SendsOneMessageAndPushesNothing()
        {
            var fixture = Create();

            fixture.Sync.Changed(fixture.Value, null, fixture.Value.ToModel());

            Assert.False(fixture.UndoManager.CanUndo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Changed_WhenTheMessengerIsBlocked_SendsNothing()
        {
            var fixture = Create();
            fixture.Messenger.IsSendingBlocked = true;

            fixture.Sync.Changed(fixture.Value, null, fixture.Value.ToModel());

            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void Changed_DuringAScope_SendsAtMostOncePerIntervalAndRecordsNothingUntilTheEnd()
        {
            var fixture = Create();
            var sync = (ValueSync)fixture.Sync;
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();
            long now = 1000;
            sync.Timestamp = () => now;

            try
            {
                interaction.BeginScope();
                for (int i = 0; i < 100; i++)
                    sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

                Assert.Single(fixture.Sender.Sent);
                Assert.False(fixture.UndoManager.CanUndo);

                now += Stopwatch.Frequency / 60;
                sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

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
            var sync = (ValueSync)fixture.Sync;
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();
            long now = 1000;
            sync.Timestamp = () => now;

            try
            {
                using (interaction.BeginScope())
                {
                    sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());
                    sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());
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
            var sync = (ValueSync)fixture.Sync;
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();
            long now = 1000;
            sync.Timestamp = () => now;

            try
            {
                using (interaction.BeginScope())
                    sync.Changed(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

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
            var sync = (ValueSync)fixture.Sync;
            var interaction = fixture.Provider.GetRequiredService<ValueInteraction>();
            long now = 1000;
            sync.Timestamp = () => now;

            try
            {
                using (interaction.BeginScope())
                {
                    sync.Changed(fixture.Value, null, fixture.Value.ToModel());
                    sync.Changed(fixture.Value, null, fixture.Value.ToModel());
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
            var factory = Create().Provider.GetRequiredService<ValueSyncFactory>();

            var first = factory();
            var second = factory();

            Assert.NotSame(first, second);
        }
    }
}
