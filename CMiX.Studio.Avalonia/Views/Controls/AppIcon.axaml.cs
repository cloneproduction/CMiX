// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons.Avalonia;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class AppIcon : UserControl
    {
        private readonly MaterialIcon _materialIcon;

        public AppIcon()
        {
            InitializeComponent();
            _materialIcon = this.FindControl<MaterialIcon>("materialIcon")!;
        }

        public static readonly StyledProperty<string> IconKeyProperty =
            AvaloniaProperty.Register<AppIcon, string>(nameof(IconKey));
        public string IconKey
        {
            get => GetValue(IconKeyProperty);
            set => SetValue(IconKeyProperty, value);
        }

        public static readonly StyledProperty<double> SizeProperty =
            AvaloniaProperty.Register<AppIcon, double>(nameof(Size));
        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        static AppIcon()
        {
            IconKeyProperty.Changed.AddClassHandler<AppIcon>((appIcon, e) => appIcon.OnIconKeyChanged(e));
        }

        // Fail loudly on an unknown key - a typo should break a build/test, not
        // silently render a blank icon.
        private void OnIconKeyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var key = e.NewValue as string;
            if (string.IsNullOrEmpty(key))
                return;

            _materialIcon.Kind = Themes.Icons.Kinds[key];
        }
    }
}
