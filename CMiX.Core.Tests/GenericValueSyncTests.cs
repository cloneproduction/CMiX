// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Xunit;

namespace CMiX.Core.Tests
{
    // What a GenericValue asks its sync to do.
    public class GenericValueSyncTests
    {
        [Fact]
        public void Value_SetWhenActive_ChangingThenChanged()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();

            value.Value = 2f;

            Assert.Equal(new[] { "Changing", "Changed" }, sync.Calls);
            Assert.Equal(2f, value.Value);
        }

        [Fact]
        public void Value_SetToTheSameValue_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();

            value.Value = 0f;

            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Value_SetToTheSameValueDuringADrag_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();
            value.Value = 1f;

            using (value.BeginDrag())
            {
                sync.Calls.Clear();
                value.Value = 1f;

                Assert.Empty(sync.Calls);
                value.Value = 2f;
            }

            Assert.Equal(new[] { "Changing", "Changed", "EndInteraction" }, sync.Calls);
        }

        [Fact]
        public void Value_SetWhenInactive_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = new GenericValue<float>(sync, new ControlRegistry());

            value.Value = 2f;

            Assert.Equal(2f, value.Value);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Value_SetWhenMomentary_SendsOnly()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();
            value.IsMomentary = true;

            value.Value = 1f;

            Assert.Equal(new[] { "Send" }, sync.Calls);
            Assert.Equal(1f, value.Value);
        }

        [Fact]
        public void BeginDrag_OfAValue_OpensAndClosesTheDragOnItsSync()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();

            using (value.BeginDrag()) { }

            Assert.Equal(new[] { "BeginInteraction", "EndInteraction" }, sync.Calls);
        }

        [Fact]
        public void BeginDrag_OfAModulatableValue_OpensAndClosesTheDragOnTheSyncOfItsValue()
        {
            var sync = new FakeValueSync();
            var other = new FakeValueSync();
            var value = new ModulatableValue<float>(sync.CreateActive<float>(), other.CreateActive<Guid?>(), other.CreateActive<string>(), new UndoManager(TimeProvider.System));

            using (value.BeginDrag()) { }

            Assert.Equal(new[] { "BeginInteraction", "EndInteraction" }, sync.Calls);
            Assert.Empty(other.Calls);
        }

        [Fact]
        public void Receive_OfAMessageWithItsId_SetsTheValueAndCallsNothing()
        {
            var sync = new FakeValueSync();
            var value = sync.CreateActive<float>();
            var message = new MessageValueChanged(value.ID, new GenericValueModel<float> { ID = value.ID, Value = 3f });

            value.Receive(message);

            Assert.Equal(3f, value.Value);
            Assert.Empty(sync.Calls);
        }
    }
}
