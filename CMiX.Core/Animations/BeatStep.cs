// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    // One value of the list of a BeatStepModulator.
    public class BeatStep : GenericValue<float>, IControl
    {
        public BeatStep(IValueSync sync, ControlActivationService activationService)
            : base(sync, activationService)
        {
        }

        protected override IControlModel CaptureModel() => ToModel();

        public new IControlModel ToModel() => new BeatStepModel
        {
            ID = ID,
            Value = Value
        };

        public new void FromModel(IControlModel model)
        {
            var m = (BeatStepModel)model;
            ID = m.ID;
            Value = m.Value;
        }
    }
}
