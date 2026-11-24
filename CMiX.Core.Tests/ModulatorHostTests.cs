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
    public class ModulatorHostTests
    {
        [Fact]
        public void Add_OfTheSameBindableTwice_HoldsItOnce()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            using var host = provider.GetRequiredService<ModulatorHost>();

            host.Add(blur.Strength);
            host.Add(new IModulatorBindable[] { blur.Strength });

            Assert.Single(host.Bindables);
        }

        [Fact]
        public void Rebind_OfABoundValueAfterLoad_BindsItToTheOutputAgain()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            using var host = provider.GetRequiredService<ModulatorHost>();
            host.Manager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)host.Manager.ManagerData.Items[0];
            blur.Strength.ModulatorID = modulator.ID;
            blur.Strength.BoundOutputName = modulator.Outputs[0].Name;
            host.Add(blur.Strength);

            host.Rebind();

            Assert.Same(modulator, blur.Strength.BoundModulator);
        }
    }
}
