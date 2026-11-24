// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every filter binds its values to the modulators of its own list. A new filter is found by reflection.
    public class TextureFilterBaseTests
    {
        public static IEnumerable<object[]> FilterTypes() =>
            typeof(TextureFilterBase).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(TextureFilterBase).IsAssignableFrom(t))
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .Select(t => new object[] { t });

        private static TextureFilterBase Create(Type type) =>
            (TextureFilterBase)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

        private static T AddModulator<T>(TextureFilterBase filter) where T : IModulator
        {
            filter.ModulatorManager.AddItem(typeof(T));
            return (T)filter.ModulatorManager.ManagerData.Items[^1];
        }

        private static void Bind(ModulatableValue<float> value, IModulator modulator) =>
            value.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

        [Fact]
        public void FilterTypes_OfTheCoreAssembly_HoldEveryFilter()
        {
            var types = FilterTypes().Select(row => (Type)row[0]).ToList();

            Assert.True(types.Count >= 21, $"Only {types.Count} filter types were found.");
            Assert.Contains(typeof(Blur), types);
            Assert.Contains(typeof(SetAlpha), types);
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void SetModulatorCommand_OnEveryValue_BindsTheModulator(Type type)
        {
            var filter = Create(type);
            var modulator = AddModulator<RandomModulator>(filter);

            foreach (var value in filter.Bindables)
                Bind(value, modulator);

            Assert.All(filter.Bindables, value =>
            {
                Assert.Equal(modulator.ID, value.ModulatorID);
                Assert.Same(modulator, value.BoundModulator);
            });
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FromModel_OfABoundFilter_BindsEveryValueToTheLoadedModulator(Type type)
        {
            var filter = Create(type);
            var modulator = AddModulator<RandomModulator>(filter);
            foreach (var value in filter.Bindables)
                Bind(value, modulator);

            var loaded = Create(type);
            loaded.FromModel(filter.ToModel());

            var loadedModulator = Assert.IsType<RandomModulator>(Assert.Single(loaded.ModulatorManager.ManagerData.Items));
            Assert.Equal(filter.Bindables.Count, loaded.Bindables.Count);
            Assert.All(loaded.Bindables, value => Assert.Same(loadedModulator, value.BoundModulator));
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void RemoveItem_OfABoundModulator_UnbindsEveryValue(Type type)
        {
            var filter = Create(type);
            var modulator = AddModulator<RandomModulator>(filter);
            foreach (var value in filter.Bindables)
                Bind(value, modulator);

            filter.ModulatorManager.RemoveItem(modulator);

            Assert.All(filter.Bindables, value =>
            {
                Assert.Null(value.ModulatorID);
                Assert.Null(value.BoundModulator);
            });
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void Move_OfTwoBoundModulators_KeepsEveryBinding(Type type)
        {
            var filter = Create(type);
            var first = AddModulator<BeatRandomModulator>(filter);
            var second = AddModulator<BeatRandomModulator>(filter);
            for (var i = 0; i < filter.Bindables.Count; i++)
                Bind(filter.Bindables[i], i % 2 == 0 ? first : second);

            filter.ModulatorManager.ManagerData.Items.Move(0, 1);

            for (var i = 0; i < filter.Bindables.Count; i++)
            {
                var expected = i % 2 == 0 ? first : second;
                Assert.Equal(expected.ID, filter.Bindables[i].ModulatorID);
                Assert.Same(expected, filter.Bindables[i].BoundModulator);
            }
        }
    }
}
