// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class BadgesKindCount : UserControl
    {
        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<BadgesKindCount, bool>(nameof(IsSelected));

        public static readonly StyledProperty<bool> IsHoveredProperty =
            AvaloniaProperty.Register<BadgesKindCount, bool>(nameof(IsHovered));

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool IsHovered
        {
            get => GetValue(IsHoveredProperty);
            set => SetValue(IsHoveredProperty, value);
        }

        static BadgesKindCount()
        {
            IsSelectedProperty.Changed.AddClassHandler<BadgesKindCount>((badges, e) => badges.PseudoClasses.Set(":selected", e.GetNewValue<bool>()));
            IsHoveredProperty.Changed.AddClassHandler<BadgesKindCount>((badges, e) => badges.PseudoClasses.Set(":hovered", e.GetNewValue<bool>()));
        }

        public BadgesKindCount()
        {
            InitializeComponent();
        }
    }
}
