// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation.Modulators
{
    public interface IModulatorOutput
    {
        string Name { get; }
    }

    public record ModulatorOutput<T>(string Name) : IModulatorOutput;
}
