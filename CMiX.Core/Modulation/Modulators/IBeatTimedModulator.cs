// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation.Modulators
{
    // What BeatControls binds to.
    public interface IBeatTimedModulator
    {
        GenericValue<int> BeatIndex { get; }

        MasterBeat MasterBeat { get; }

        float BPM { get; }

        ICommand MultiplyCommand { get; }

        ICommand DivideCommand { get; }
    }
}
