// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation.Modulators
{
    // An LFO whose period is a beat of the master beat, not a time in seconds.
    public class BeatLFOModulator : BeatTimedModulatorBase
    {
        public BeatLFOModulator(PrefabService prefabService,
                                MasterBeat masterBeat,
                                GenericValue<int> beatIndex,
                                GenericValue<float> center,
                                GenericValue<float> width,
                                Easing easing,
                                BeatSteps beatSteps,
                                UndoManager undoManager,
                                ControlActivationService activationService)
            : base(prefabService, masterBeat, beatIndex, center, width, easing, beatSteps, undoManager, activationService)
        {
        }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] { new ModulatorOutput<float>("Value") };

        public override IControlModel ToModel() => new BeatLFOModulatorModel
        {
            ID = ID,
            Easing = (EasingModel)Easing.ToModel(),
            BeatIndex = (GenericValueModel<int>)BeatIndex.ToModel(),
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatSteps = (BeatStepsModel)BeatSteps.ToModel(),
            Center = (GenericValueModel<float>)Center.ToModel(),
            Width = (GenericValueModel<float>)Width.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (BeatLFOModulatorModel)model;
            ID = m.ID;
            Easing.FromModel(m.Easing);
            BeatIndex.FromModel(m.BeatIndex);
            PrefabService.FromModel(m.PrefabService);
            BeatSteps.FromModel(m.BeatSteps);
            Center.FromModel(m.Center);
            Width.FromModel(m.Width);
        }
    }
}
