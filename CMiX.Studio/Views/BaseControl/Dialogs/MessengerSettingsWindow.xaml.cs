// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Studio.Views.BaseControl;
using System;
using System.Windows;
using System.Windows.Input;

namespace CMiX.Studio.Views
{
    public partial class MessengerSettingsWindow : Window
    {
        public MessengerSettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ipBox.txtboxFirstPart.Focus();
            MoveBottomRightEdgeOfWindowToMousePosition();
        }

        private void MoveBottomRightEdgeOfWindowToMousePosition()
        {
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(GetMousePosition());
            Left = mouse.X - ActualWidth * 0.5;
            Top = mouse.Y - 25;
        }

        public Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        bool isClosing = false;

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!isClosing)
                Close();
        }

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
