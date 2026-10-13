// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    public abstract class UserControlModulatable : UserControlCaptioned
    {
        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<UserControlModulatable, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }
    }
}
