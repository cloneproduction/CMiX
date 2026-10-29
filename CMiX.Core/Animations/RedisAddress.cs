// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Animations
{
    // One Redis address of the list. A Redis modulator selects one of them.
    public class RedisAddress : GenericValue<string>, IControl
    {
        public RedisAddress(ControlMessenger controlMessenger,
                            MessageFactory messageFactory,
                            ControlActivationService activationService,
                            UndoManager undoManager)
            : base(controlMessenger,
                   messageFactory,
                   activationService,
                   undoManager)
        {
        }

        protected override IControlModel CaptureModel() => ToModel();

        public new IControlModel ToModel() => new RedisAddressModel
        {
            ID = ID,
            Value = Value
        };

        public new void FromModel(IControlModel model)
        {
            var m = (RedisAddressModel)model;
            ID = m.ID;
            Value = m.Value;
        }
    }
}
