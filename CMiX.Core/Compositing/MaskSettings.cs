// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class MaskSettings : IControl
    {
        public MaskSettings(GenericValue<bool> isMask,
                            GenericValue<MaskChannel> maskChannel,
                            GenericValue<MaskMode> maskMode,
                            GenericValue<bool> invert)
        {
            IsMask = isMask;
            MaskChannel = maskChannel;
            MaskMode = maskMode;
            Invert = invert;
        }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public Guid ID { get; set; }

        public void FromModel(IControlModel model)
        {
            var m = (MaskSettingsModel)model;
            ID = m.ID;
            IsMask.FromModel(m.IsMask);
            MaskChannel.FromModel(m.MaskChannel);
            MaskMode.FromModel(m.MaskMode);
            Invert.FromModel(m.Invert);
        }

        public IControlModel ToModel() => new MaskSettingsModel
        {
            ID = ID,
            IsMask = (GenericValueModel<bool>)IsMask.ToModel(),
            MaskChannel = (GenericValueModel<MaskChannel>)MaskChannel.ToModel(),
            MaskMode = (GenericValueModel<MaskMode>)MaskMode.ToModel(),
            Invert = (GenericValueModel<bool>)Invert.ToModel()
        };
    }
}
