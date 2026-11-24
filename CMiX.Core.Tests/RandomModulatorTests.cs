// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class RandomModulatorTests
    {
        [Fact]
        public void Outputs_OfANewModulator_StartWithAFloatOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulator = (RandomModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(RandomModulator));

            Assert.IsType<ModulatorOutput<float>>(modulator.Outputs[0]);
        }
    }
}
