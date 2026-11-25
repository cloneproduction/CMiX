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

        private static Fixture Create() => Create(TestServiceProviderFactory.Create());

        private static Fixture Create(IServiceProvider provider)
        {
            var undoManager = provider.GetRequiredService<UndoManager>();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            var time = new FakeTimeProvider { Now = 1000 };
            var sync = new ValueSync(undoManager, provider.GetRequiredService<MessageFactory>(), messenger, time);
            var value = new GenericValue<float>(sync, provider.GetRequiredService<ControlRegistry>());
            value.Activate();
            return new Fixture
            {
                Sync = sync,
                Time = time,
                Messenger = messenger,
                Sender = sender,
                UndoManager = undoManager,
                Value = value,
            };
        }

        [Fact]
        public void Changing_ThenChanged_PushesOneValueChangedUndoCommandAndSendsOneMessage()
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
        public void Changed_OutsideADrag_SendsEveryWrite()
        {
            var fixture = Create();

            for (int i = 1; i <= 20; i++)
                fixture.Value.Value = i;

            Assert.Equal(20, fixture.Sender.Sent.Count);
        }

        [Fact]
        public void Send_DuringADrag_IsThrottledAndRecordsNothing()
        {
            var fixture = Create();

            using (fixture.Sync.BeginDrag())
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
        public void BeginDrag_Then100Writes_SendsOnceAndRecordsNothingUntilTheEnd()
        {
            var fixture = Create();

            using (fixture.Sync.BeginDrag())
            {
                for (int i = 0; i < 100; i++)
                    fixture.Change();

                Assert.Single(fixture.Sender.Sent);
                Assert.False(fixture.UndoManager.CanUndo);

                fixture.Time.Now += 1000;
                fixture.Change();

                Assert.Equal(2, fixture.Sender.Sent.Count);
                Assert.False(fixture.UndoManager.CanUndo);
            }

            Assert.True(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void BeginDrag_ThenDispose_SendsThePendingValueOnceAndRecordsOneStep()
        {
            var fixture = Create();
            fixture.Value.Value = 1f;
            fixture.Sender.Sent.Clear();
            fixture.UndoManager.Clear();

            using (fixture.Sync.BeginDrag())
            {
                fixture.Value.Value = 2f;
                fixture.Value.Value = 3f;
            }

            Assert.Equal(2, fixture.Sender.Sent.Count);
            fixture.UndoManager.Undo();
            Assert.Equal(1f, fixture.Value.Value);
            Assert.False(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void BeginDrag_ThenManyWrites_SendsTheFinalValueLast()
        {
            var fixture = Create();

            using (fixture.Sync.BeginDrag())
            {
                for (int i = 1; i <= 500; i++)
                    fixture.Value.Value = i;
            }

            var values = fixture.Sender.Sent.Select(message => ((GenericValueModel<float>)((MessageValueChanged)message).Value).Value).ToList();
            Assert.True(values.Count < 500, $"expected throttled sends, got {values.Count}");
            Assert.Equal(500f, values[^1]);
        }

        [Fact]
        public void BeginDrag_WithNothingPending_RecordsAndSendsNothingMore()
        {
            var fixture = Create();

            using (fixture.Sync.BeginDrag())
                fixture.Change();

            fixture.AssertOneValueChanged();
            Assert.True(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void BeginDrag_OfAMomentaryValue_SendsThePendingValueAndRecordsNothing()
        {
            var fixture = Create();

            using (fixture.Sync.BeginDrag())
            {
                fixture.Sync.Send(fixture.Value);
                fixture.Sync.Send(fixture.Value);
            }

            Assert.Equal(2, fixture.Sender.Sent.Count);
            Assert.False(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void Dispose_OfAStaleHandle_EndsNothing()
        {
            var fixture = Create();
            var first = fixture.Sync.BeginDrag();
            first.Dispose();
            var second = fixture.Sync.BeginDrag();
            fixture.Change();

            first.Dispose();
            fixture.Change();

            Assert.Single(fixture.Sender.Sent);
            Assert.False(fixture.UndoManager.CanUndo);

            second.Dispose();

            Assert.Equal(2, fixture.Sender.Sent.Count);
            Assert.True(fixture.UndoManager.CanUndo);
        }

        [Fact]
        public void BeginDrag_WhileADragIsOpen_EndsTheFirstDrag()
        {
            var fixture = Create();
            fixture.Sync.BeginDrag();
            fixture.Change();
            fixture.Change();

            using (fixture.Sync.BeginDrag())
            {
                Assert.Equal(2, fixture.Sender.Sent.Count);
                Assert.True(fixture.UndoManager.CanUndo);
            }
        }

        [Fact]
        public void Changed_OfAnotherValueDuringADrag_RecordsAtOnce()
        {
            var provider = TestServiceProviderFactory.Create();
            var fixture = Create(provider);
            var other = Create(provider);

            using (fixture.Sync.BeginDrag())
            {
                fixture.Change();
                Assert.False(fixture.UndoManager.CanUndo);
                other.Change();

                Assert.True(fixture.UndoManager.CanUndo);
                var message = Assert.IsType<MessageValueChanged>(fixture.Sender.Sent.Last());
                Assert.Equal(other.Value.ID, message.ID);
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
