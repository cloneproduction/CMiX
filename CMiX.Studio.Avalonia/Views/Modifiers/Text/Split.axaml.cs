// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Data.Converters;
using CMiX.Core.Text.Modifiers;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class Split : UserControl
    {
        // Replaces the WPF DataTrigger comparing Type.Value with SplitType.Separator.
        public static readonly IValueConverter IsSeparator =
            new FuncValueConverter<SplitType, bool>(type => type == SplitType.Separator);

        public Split()
        {
            InitializeComponent();
        }
    }
}
