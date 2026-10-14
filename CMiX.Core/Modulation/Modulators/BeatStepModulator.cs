// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation.Modulators
{
    // A modulator on the beat with a list of values. The user adds, removes and moves the BeatSteps
    // of the StepManager.
    public class BeatStepModulator : BeatTimedModulatorBase
    {
        public BeatStepModulator(PrefabService prefabService,
                                 MasterBeat masterBeat,
                                 GenericValue<int> beatIndex,
                                 GenericValue<float> center,
                                 GenericValue<float> width,
                                 Easing easing,
                                 BeatSteps beatSteps,
                                 PrefabManager stepManager,
                                 UndoManager undoManager,
                                 ControlActivationService activationService)
            : base(prefabService, masterBeat, beatIndex, center, width, easing, beatSteps, undoManager, activationService)
        {
            StepManager = stepManager;
        }

        public PrefabManager StepManager { get; set; }

        public override IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[] { new ModulatorOutput<float>("Value") };

        public override void Dispose()
        {
            base.Dispose();
            StepManager.Dispose();
        }

        public override IControlModel ToModel() => new BeatStepModulatorModel
        {
            ID = ID,
            Easing = (EasingModel)Easing.ToModel(),
            BeatIndex = (GenericValueModel<int>)BeatIndex.ToModel(),
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatSteps = (BeatStepsModel)BeatSteps.ToModel(),
            StepManager = (PrefabManagerModel)StepManager.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (BeatStepModulatorModel)model;
            ID = m.ID;
            Easing.FromModel(m.Easing);
            BeatIndex.FromModel(m.BeatIndex);
            PrefabService.FromModel(m.PrefabService);
            BeatSteps.FromModel(m.BeatSteps);

            LoadManager(StepManager, m.StepManager);
        }
    }
}
