// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core
{
    public interface IControl
    {
        Guid ID { get; set; }
        IControlModel ToModel();
        void FromModel(IControlModel model);
    }
}
