// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Core.Modulation
{
    public interface IModulatorBindable
    {
        Guid? ModulatorID { get; }
        string BoundOutputName { get; }
        IModulator BoundModulator { get; }
        bool IsModulated { get; }
        ICommand SetModulatorCommand { get; }
        Func<Guid, IModulator> ModulatorLookup { get; set; }

        bool CanBind(IModulatorOutput output);
    }
}
