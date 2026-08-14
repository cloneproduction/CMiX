// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MessengerSettingsWindow : Window
    {
        public MessengerSettingsWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
            Opened += (s, e) =>
            {
                ipBox.FindControl<TextBox>("txtboxFirstPart")?.Focus();
                MoveBottomRightEdgeOfWindowToMousePosition();
            };
            Deactivated += (s, e) =>
            {
                if (!isClosing)
                    Close();
            };
        }

        // Replaces the WPF PresentationSource plus WinForms cursor positioning.
        private void MoveBottomRightEdgeOfWindowToMousePosition()
        {
            var mouse = DragEditHelper.GetMousePosition();
            var scaling = RenderScaling;
            var x = (int)(mouse.X - Bounds.Width * scaling * 0.5);
            var y = (int)(mouse.Y - 25 * scaling);
            Position = ClampToScreen(new PixelPoint(x, y));
        }

        private PixelPoint ClampToScreen(PixelPoint point)
        {
            var screen = Screens.ScreenFromPoint(point) ?? Screens.Primary;
            if (screen == null)
                return point;

            var area = screen.WorkingArea;
            var x = Math.Clamp(point.X, area.X, Math.Max(area.X, area.Right - (int)(Bounds.Width * RenderScaling)));
            var y = Math.Clamp(point.Y, area.Y, Math.Max(area.Y, area.Bottom - (int)(Bounds.Height * RenderScaling)));
            return new PixelPoint(x, y);
        }

        bool isClosing = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isClosing = true;
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                isClosing = true;
                Close();
            }
        }
    }
}
