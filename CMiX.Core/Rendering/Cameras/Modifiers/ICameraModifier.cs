// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public interface ICameraModifier
    {
        public GenericValue<CameraAxis> Axis { get; set; }
    }
}
