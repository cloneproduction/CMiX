// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The one global list of Redis addresses: its fixed id, the default name, and a remove that takes the bound values back to normal.
    public class RedisAddressTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisAddress AddAddress(string text)
        {
            Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private static RedisFloatModulator AddModulator(PrefabManager manager)
        {
            manager.AddItem(typeof(RedisFloatModulator));
            return manager.ManagerData.Items.OfType<RedisFloatModulator>().Single();
        }

        private static RedisBoolModulator AddBoolModulator(PrefabManager manager)
        {
            manager.AddItem(typeof(RedisBoolModulator));
            return manager.ManagerData.Items.OfType<RedisBoolModulator>().Single();
        }

        private static RedisIntModulator AddIntModulator(PrefabManager manager)
        {
            manager.AddItem(typeof(RedisIntModulator));
            return manager.ManagerData.Items.OfType<RedisIntModulator>().Single();
        }

        // Selects the address on the modulator and binds the value to its output.
        private static void Bind(IModulatorBindable bindable, RedisModulatorBase modulator, RedisAddress address)
        {
            modulator.SelectedAddress = address;
            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs.Single()));
        }

        private static (Project Project, UndoManager Undo) NewProject()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<Project>(), provider.GetRequiredService<UndoManager>());
        }

        private static List<string> Names(Project project) =>
            project.RedisAddressRegistry.Addresses.Select(address => address.Value).ToList();

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfAFilter_TakesTheValueBackToNormal()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(blur.Strength.ModulatorID);
            Assert.False(blur.Strength.IsModulated);
            Assert.Null(modulator.SelectedAddress);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfAModifier_TakesTheValueBackToNormal()
        {
            var scale = (ScaleModifier)_provider.GetRequiredService<ControlFactory>().Create(typeof(ScaleModifier));
            var modulator = AddModulator(scale.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(scale.X, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(scale.X.ModulatorID);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfATextureSource_TakesTheValueBackToNormal()
        {
            var source = (VideoPlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(VideoPlayer));
            var modulator = AddModulator(source.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(source.SeekTarget, modulator, address);

            Registry.RemoveAddress(address);

            Assert.Null(source.SeekTarget.ModulatorID);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfABoolModulator_TakesPlayingBackToNormal()
        {
            var player = (SequencePlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            var modulator = AddBoolModulator(player.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(player.Playing, modulator, address);
            Assert.True(player.Playing.IsModulated);

            Registry.RemoveAddress(address);

            Assert.Null(player.Playing.ModulatorID);
            Assert.False(player.Playing.IsModulated);
            Assert.Null(modulator.SelectedAddress);
        }

        [Fact]
        public void RemoveAddress_TheSelectedAddressOfAnIntModulator_TakesSeekTargetBackToNormal()
        {
            var player = (SequencePlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            var modulator = AddIntModulator(player.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(player.SeekTarget, modulator, address);
            Assert.True(player.SeekTarget.IsModulated);

            Registry.RemoveAddress(address);

            Assert.Null(player.SeekTarget.ModulatorID);
            Assert.False(player.SeekTarget.IsModulated);
            Assert.Null(modulator.SelectedAddress);
        }

        [Fact]
        public void RemoveAddress_AnotherAddress_KeepsTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var removed = AddAddress("cmix:a");
            var kept = AddAddress("cmix:b");
            Bind(blur.Strength, modulator, kept);

            Registry.RemoveAddress(removed);

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.Same(kept, modulator.SelectedAddress);
        }

        [Fact]
        public void ValueAndMoveItemTo_OfASelectedAddress_KeepTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var first = AddAddress("cmix:a");
            AddAddress("cmix:b");
            Bind(blur.Strength, modulator, first);

            first.Value = "cmix:changed";
            Registry.AddressManager.MoveItemTo(first, 1);

            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.True(blur.Strength.IsModulated);
            Assert.Same(first, modulator.SelectedAddress);
        }

        [Fact]
        public void SelectedAddress_ChangedToAnotherAddress_KeepsTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var first = AddAddress("cmix:a");
            var second = AddAddress("cmix:b");
            Bind(blur.Strength, modulator, first);

            modulator.SelectedAddress = second;

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
            Assert.True(blur.Strength.IsModulated);
        }

        [Fact]
        public void RemoveAddress_AfterTheModulatorIsRemoved_StillWorks()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            blur.ModulatorManager.RemoveItem(modulator);

            Registry.RemoveAddress(address);

            Assert.Empty(Registry.Addresses);
            Assert.Null(blur.Strength.ModulatorID);
        }

        [Fact]
        public void Undo_OfRemoveAddress_BringsBackTheAddressTheSelectionAndTheBinding()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            var modulator = AddModulator(blur.ModulatorManager);
            var address = AddAddress("cmix:a");
            Bind(blur.Strength, modulator, address);
            _provider.GetRequiredService<UndoManager>().Clear();

            Registry.RemoveAddress(address);
            _provider.GetRequiredService<UndoManager>().Undo();

            Assert.Single(Registry.Addresses);
            Assert.Equal(address.ID, modulator.SelectedAddressID.Value);
            Assert.NotNull(modulator.SelectedAddress);
            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
            Assert.Equal("Value", blur.Strength.BoundOutputName);
        }

        [Fact]
        public void AddressManager_OfTheRegistry_HasTheFixedID()
        {
            Assert.Equal(ManagerIDs.RedisAddressManager, Registry.AddressManager.ManagerData.ID);
        }

        [Fact]
        public void Add_AnAddress_NamesItAddressWithANumber()
        {
            var (project, _) = NewProject();

            project.RedisAddressRegistry.AddAddress();
            project.RedisAddressRegistry.AddAddress();

            Assert.Equal(new[] { "Address.001", "Address.002" }, Names(project));
        }

        [Fact]
        public void Add_AnAddressAfterOthers_TakesTheFirstFreeNumber()
        {
            var (project, _) = NewProject();
            project.RedisAddressRegistry.AddAddress();
            project.RedisAddressRegistry.AddAddress();
            project.RedisAddressRegistry.AddAddress();
            project.RedisAddressRegistry.RemoveAddress(project.RedisAddressRegistry.Addresses.ElementAt(1));

            project.RedisAddressRegistry.AddAddress();

            Assert.Equal(new[] { "Address.001", "Address.003", "Address.002" }, Names(project));
        }

        [Fact]
        public void Add_AnAddress_IsOneUndoStepWithItsName()
        {
            var (project, undo) = NewProject();
            undo.Clear();

            project.RedisAddressRegistry.AddAddress();
            undo.Undo();

            Assert.Empty(Names(project));
        }
    }
}
