// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Whether a menu of the new tree is open: the context menu of the host or the Add panel.
    internal sealed class ListTreeMenuOpen
    {
        private readonly Control host;
        private readonly AddPanel addPanel;

        public ListTreeMenuOpen(Control host, AddPanel addPanel)
        {
            this.host = host;
            this.addPanel = addPanel;
        }

        public bool IsMenuOpen() => host.ContextMenu?.IsOpen == true || addPanel.IsOpen;
    }
}
