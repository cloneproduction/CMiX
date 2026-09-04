// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation.Modulators
{
    public interface IBeatTimedModulator
    {
        GenericValue<int> BeatIndex { get; }

        MasterBeat MasterBeat { get; }
    }
}
