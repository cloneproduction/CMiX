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
        public void Commit_OfAChange_PushesOneValueChangedCommandAndSendsOneMessage()
        {
            var fixture = Create();

            fixture.Sync.Commit(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

            Assert.True(fixture.UndoManager.CanUndo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Commit_WhileTheUndoManagerApplies_SendsButPushesNothing()
        {
            var fixture = Create();
            fixture.UndoManager.Push(new ActionUndoCommand(() =>
                fixture.Sync.Commit(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel())));

            fixture.UndoManager.Undo();

            Assert.False(fixture.UndoManager.CanUndo);
            Assert.True(fixture.UndoManager.CanRedo);
            var message = Assert.IsType<MessageValueChanged>(Assert.Single(fixture.Sender.Sent));
            Assert.Equal(fixture.Value.ID, message.ID);
        }

        [Fact]
        public void Record_OfAChange_PushesOneValueChangedCommandAndSendsNothing()
        {
            var fixture = Create();

            fixture.Sync.Record(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel());

            Assert.True(fixture.UndoManager.CanUndo);
            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void Record_WhileTheUndoManagerApplies_PushesNothing()
        {
            var fixture = Create();
            fixture.UndoManager.Push(new ActionUndoCommand(() =>
                fixture.Sync.Record(fixture.Value, fixture.Value.ToModel(), fixture.Value.ToModel())));

            fixture.UndoManager.Undo();

            Assert.False(fixture.UndoManager.CanUndo);
            Assert.True(fixture.UndoManager.CanRedo);
            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void Send_OfAValue_SendsOneMessageAndPushesNothing()
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
        public void ValueSyncFactory_Called_GivesANewSyncEachTime()
        {
            var factory = Create().Provider.GetRequiredService<ValueSyncFactory>();

            var first = factory();
            var second = factory();

            Assert.NotSame(first, second);
        }
    }
}
