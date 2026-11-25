// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ProjectSyncTargetTests
    {
        // The target builds the project, and the project activates every pending control. So the target comes first.
        private static (IServiceProvider provider, ISyncTarget target) NewGraph()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider, provider.GetRequiredService<ISyncTarget>());
        }

        private static MessageValueChanged ValueMessage(Guid id, float value) =>
            new(id, new GenericValueModel<float> { ID = id, Value = value });

        [Fact]
        public void Apply_OfAValueMessage_ReachesTheValueWithThatIdOnly()
        {
            var (provider, target) = NewGraph();
            var first = provider.GetRequiredService<GenericValue<float>>();
            var second = provider.GetRequiredService<GenericValue<float>>();
            provider.GetRequiredService<ControlRegistry>().ActivateAll();

            target.Apply(ValueMessage(first.ID, 3f));

            Assert.Equal(3f, first.Value);
            Assert.Equal(0f, second.Value);
        }

        [Fact]
        public void Apply_OfAMessageForAnIdSetAfterActivation_ReachesTheControl()
        {
            var (provider, target) = NewGraph();
            var value = provider.GetRequiredService<GenericValue<float>>();
            provider.GetRequiredService<ControlRegistry>().ActivateAll();
            value.ID = Guid.NewGuid();

            target.Apply(ValueMessage(value.ID, 3f));

            Assert.Equal(3f, value.Value);
        }

        [Fact]
        public void Apply_OfAMessageForAnInactiveControl_ChangesNothing()
        {
            var (provider, target) = NewGraph();
            var value = provider.GetRequiredService<GenericValue<float>>();

            target.Apply(ValueMessage(value.ID, 3f));

            Assert.Equal(0f, value.Value);
        }

        [Fact]
        public void Apply_OfAListMessage_ReachesTheManagerByItsManagerDataId()
        {
            var (provider, target) = NewGraph();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.Activate();
            var id = new Guid("7d1f6c2e-3b4a-4c5d-9e8f-0a1b2c3d4e5f");
            manager.ManagerData.ID = id;
            var source = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            source.CompositionManager.AddItem(typeof(Composition));
            var model = source.CompositionManager.ManagerData.Items[0].ToModel();

            target.Apply(new MessageAddItem(id, model, 0));

            var item = Assert.Single(manager.ManagerData.Items);
            Assert.Equal(model.ID, item.ID);
        }

        [Fact]
        public void Apply_OfAMessageForAnUnknownId_DoesNothing()
        {
            var (provider, target) = NewGraph();
            var value = provider.GetRequiredService<GenericValue<float>>();
            provider.GetRequiredService<ControlRegistry>().ActivateAll();

            target.Apply(ValueMessage(Guid.NewGuid(), 3f));

            Assert.Equal(0f, value.Value);
        }

        [Fact]
        public void Apply_OfAValueMessage_SendsNothingBackAndRecordsNoUndoStep()
        {
            var (provider, target) = NewGraph();
            var value = provider.GetRequiredService<GenericValue<float>>();
            provider.GetRequiredService<ControlRegistry>().ActivateAll();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);
            var undo = provider.GetRequiredService<UndoManager>();
            var steps = undo.StepCount;

            target.Apply(ValueMessage(value.ID, 3f));

            Assert.Equal(3f, value.Value);
            Assert.Empty(sender.Sent);
            Assert.Equal(steps, undo.StepCount);
        }
    }
}
