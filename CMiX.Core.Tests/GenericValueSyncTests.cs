// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using Xunit;

namespace CMiX.Core.Tests
{
    // What a GenericValue asks its sync to do.
    public class GenericValueSyncTests
    {
        private static GenericValue<float> CreateActiveFloat(FakeValueSync sync)
        {
            var value = new GenericValue<float>(sync, new ControlActivationService());
            value.Activate();
            return value;
        }

        [Fact]
        public void Value_SetWhenActive_CommitsOnce()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);

            value.Value = 2f;

            Assert.Equal(new[] { "Commit" }, sync.Calls);
            Assert.Equal(1, sync.Commits);
            Assert.Equal(0f, ((GenericValueModel<float>)sync.LastBefore).Value);
            Assert.Equal(2f, ((GenericValueModel<float>)sync.LastAfter).Value);
        }

        [Fact]
        public void Value_SetToTheSameValue_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);

            value.Value = 0f;

            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Value_SetWhenInactive_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = new GenericValue<float>(sync, new ControlActivationService());

            value.Value = 2f;

            Assert.Equal(2f, value.Value);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Value_SetUnderSuppressSending_CallsNothing()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);

            using (value.SuppressSending())
                value.Value = 2f;

            Assert.Equal(2f, value.Value);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Value_SetWhenMomentary_SendsAndRecordsNothing()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);
            value.IsMomentary = true;

            value.Value = 1f;

            Assert.Equal(1, sync.Sends);
            Assert.Equal(0, sync.Records);
            Assert.Equal(0, sync.Commits);
        }

        [Fact]
        public void Value_SetDuringAnInteraction_SendsAtMostOnePerIntervalAndRecordsOnceAtTheEnd()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);

            try
            {
                ValueInteraction.BeginScope();
                for (int i = 1; i <= 100; i++)
                    value.Value = i;
            }
            finally
            {
                ValueInteraction.End();
            }

            Assert.Equal(100f, value.Value);
            Assert.Equal(1, sync.Records);
            Assert.Equal(0, sync.Commits);
            Assert.InRange(sync.Sends, 1, 100);
            Assert.Equal("Record", sync.Calls[^1]);
            Assert.Equal(0f, ((GenericValueModel<float>)sync.LastBefore).Value);
            Assert.Equal(100f, ((GenericValueModel<float>)sync.LastAfter).Value);
        }

        [Fact]
        public void Receive_OfAMessageWithItsId_SetsTheValueAndCallsNothing()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);
            var message = new MessageValueChanged(value.ID, new GenericValueModel<float> { ID = value.ID, Value = 3f });

            value.Receive(message);

            Assert.Equal(3f, value.Value);
            Assert.Empty(sync.Calls);
        }
    }
}
