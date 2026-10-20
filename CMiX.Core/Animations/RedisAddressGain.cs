// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Animations
{
    // The multiplier of one Redis address in one RedisAddressModulator.
    public class RedisAddressGain : GenericValue<float>, IControl
    {
        public RedisAddressGain(ControlMessenger controlMessenger,
                                MessageFactory messageFactory,
                                ControlActivationService activationService,
                                UndoManager undoManager,
                                GenericValue<Guid?> addressID)
            : base(controlMessenger,
                   messageFactory,
                   activationService,
                   undoManager)
        {
            AddressID = addressID;
        }

        public GenericValue<Guid?> AddressID { get; set; }

        private RedisAddress _address;
        public RedisAddress Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        protected override IControlModel CaptureModel() => ToModel();

        public new IControlModel ToModel() => new RedisAddressGainModel
        {
            ID = ID,
            Value = Value,
            AddressID = (GenericValueModel<Guid?>)AddressID.ToModel()
        };

        public new void FromModel(IControlModel model)
        {
            var m = (RedisAddressGainModel)model;
            ID = m.ID;
            Value = m.Value;
            AddressID.FromModel(m.AddressID);
        }
    }
}
