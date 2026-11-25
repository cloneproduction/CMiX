// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The collected control test forces a full GC. It runs alone, so it does not stall the timing of the sync tests.
    [CollectionDefinition(nameof(ControlRegistryTests), DisableParallelization = true)]
    [Collection(nameof(ControlRegistryTests))]
    public class ControlRegistryTests
    {
        [Fact]
        public void Find_OfARegisteredActiveControl_ReturnsIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<ControlRegistry>();
            var value = provider.GetRequiredService<GenericValue<float>>();

            registry.ActivateAll();

            Assert.Same(value, registry.Find(value.ID));
        }

        [Fact]
        public void Find_OfAnInactiveControl_ReturnsNull()
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<ControlRegistry>();
            var value = provider.GetRequiredService<GenericValue<float>>();

            Assert.Null(registry.Find(value.ID));
        }

        [Fact]
        public void Find_AfterTheIdChanged_ReturnsItUnderTheNewIdOnly()
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<ControlRegistry>();
            var value = provider.GetRequiredService<GenericValue<float>>();
            registry.ActivateAll();
            var oldId = value.ID;
            var newId = Guid.NewGuid();

            value.ID = newId;

            Assert.Same(value, registry.Find(newId));
            Assert.Null(registry.Find(oldId));
        }

        [Fact]
        public void Find_OfACollectedControl_ReturnsNull()
        {
            var registry = new ControlRegistry();
            var id = BuildActiveValue(registry);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Null(registry.Find(id));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Guid BuildActiveValue(ControlRegistry registry)
        {
            var value = new GenericValue<float>(new FakeValueSync(), registry);
            registry.ActivateAll();
            Assert.Same(value, registry.Find(value.ID));
            return value.ID;
        }

        [Fact]
        public void Rekey_OfAManager_UsesTheManagerDataId()
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<ControlRegistry>();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.Activate();
            var oldId = manager.ManagerData.ID;

            Assert.Same(manager, registry.Find(oldId));

            var newId = Guid.NewGuid();
            manager.ManagerData.ID = newId;

            Assert.Same(manager, registry.Find(newId));
            Assert.Null(registry.Find(oldId));
        }

        [Fact]
        public void Find_OfAButton_ReturnsItFromTheStart()
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<ControlRegistry>();
            var button = provider.GetRequiredService<CMiXButton>();

            Assert.Same(button, registry.Find(button.ID));
        }
    }
}
