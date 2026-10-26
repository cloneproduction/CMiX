// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Components
{
    // Says if a control takes part in the update and the render. It holds data only.
    public class VisibilityComponent
    {
        public VisibilityComponent(Guid controlId, GenericValue<bool> visible)
        {
            ControlId = controlId;
            Visible = visible;
        }

        public Guid ControlId { get; }
        public GenericValue<bool> Visible { get; }
        public bool IsVisible => Visible.Value;
    }
}
