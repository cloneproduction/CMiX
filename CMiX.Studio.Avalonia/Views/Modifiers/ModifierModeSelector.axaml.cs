// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Data.Converters;
using CMiX.Core.Modifiers;

namespace CMiX.Studio.Avalonia.Views.Modifiers
{
    public partial class ModifierModeSelector : UserControl
    {
        // Replaces the WPF DataTrigger comparing Mode.Value with ModifierMode.ToSpread.
        public static readonly IValueConverter IsToSpread =
            new FuncValueConverter<ModifierMode, bool>(mode => mode == ModifierMode.ToSpread);

        public ModifierModeSelector()
        {
            InitializeComponent();
        }
    }
}
