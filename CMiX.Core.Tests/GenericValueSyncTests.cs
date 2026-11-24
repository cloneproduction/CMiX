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
        public void Value_SetWhenActive_ChangesOnceWithBeforeAndAfter()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);

            value.Value = 2f;

            Assert.Equal(new[] { "Changed" }, sync.Calls);
            Assert.Equal(1, sync.Changes);
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
        public void Value_SetWhenMomentary_ChangesWithANullBefore()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);
            value.IsMomentary = true;

            value.Value = 1f;

            Assert.Equal(1, sync.Changes);
            Assert.Null(sync.LastBefore);
            Assert.Equal(1f, ((GenericValueModel<float>)sync.LastAfter).Value);
        }

        [Fact]
        public void Value_SetManyTimes_ChangesOnEveryWrite()
        {
            var sync = new FakeValueSync();
            var value = CreateActiveFloat(sync);
            IControlModel firstBefore = null;

            for (int i = 1; i <= 10; i++)
            {
                value.Value = i;
                if (i == 1)
                    firstBefore = sync.LastBefore;
            }

            Assert.Equal(10, sync.Changes);
            Assert.Equal(0f, ((GenericValueModel<float>)firstBefore).Value);
            Assert.Equal(10f, ((GenericValueModel<float>)sync.LastAfter).Value);
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
