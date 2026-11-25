// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CMiXButtonTests
    {
        [Fact]
        public void OnClick_Raises_ClickAndSendsOnce()
        {
            var sync = new FakeValueSync();
            var button = new CMiXButton(sync, new ControlRegistry());
            var clicks = 0;
            button.Click += (_, _) => clicks++;

            button.OnClick();

            Assert.Equal(1, clicks);
            Assert.Equal(new[] { "Send" }, sync.Calls);
        }

        [Fact]
        public void Receive_OfAValueChangedWithItsId_RaisesClick()
        {
            var sync = new FakeValueSync();
            var button = new CMiXButton(sync, new ControlRegistry());
            var clicks = 0;
            button.Click += (_, _) => clicks++;

            button.Receive(new MessageValueChanged(button.ID, button.ToModel()));

            Assert.Equal(1, clicks);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void Apply_OfAValueChangedForAnotherId_RaisesNoClick()
        {
            var provider = TestServiceProviderFactory.Create();
            var button = provider.GetRequiredService<CMiXButton>();
            var clicks = 0;
            button.Click += (_, _) => clicks++;

            provider.GetRequiredService<ISyncTarget>().Apply(new MessageValueChanged(Guid.NewGuid(), button.ToModel()));

            Assert.Equal(0, clicks);
        }

        [Fact]
        public void OnClick_OnTheRealGraph_PutsOneValueChangedWithTheButtonIdOnTheWire()
        {
            var provider = TestServiceProviderFactory.Create();
            var button = provider.GetRequiredService<CMiXButton>();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);

            button.OnClick();

            var message = Assert.Single(sender.Sent);
            var change = Assert.IsType<MessageValueChanged>(message);
            Assert.Equal(button.ID, change.ID);
        }

        [Fact]
        public void Apply_OfTheClickOnAnotherPeer_RaisesClickThere()
        {
            var providerA = TestServiceProviderFactory.Create();
            var providerB = TestServiceProviderFactory.Create();
            var a = providerA.GetRequiredService<CMiXButton>();
            var b = providerB.GetRequiredService<CMiXButton>();
            b.ID = a.ID;
            var sender = new RecordingMessageSender();
            providerA.GetRequiredService<ControlMessenger>().Register(sender);
            var clicksB = 0;
            b.Click += (_, _) => clicksB++;

            a.OnClick();
            providerB.GetRequiredService<ISyncTarget>().Apply(Assert.Single(sender.Sent));

            Assert.Equal(1, clicksB);
        }
    }
}
