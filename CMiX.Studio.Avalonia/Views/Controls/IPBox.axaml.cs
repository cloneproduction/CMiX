// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class IPBox : CaptionedUserControl
    {
        public IPBox()
        {
            InitializeComponent();

            // WPF PreviewKeyDown becomes a tunneling KeyDown handler.
            txtboxFirstPart.AddHandler(KeyDownEvent, txtboxFirstPart_PreviewKeyDown, RoutingStrategies.Tunnel);
        }

        public static readonly StyledProperty<string> IPAddressProperty =
            AvaloniaProperty.Register<IPBox, string>(nameof(IPAddress));
        public string IPAddress
        {
            get => GetValue(IPAddressProperty);
            set => SetValue(IPAddressProperty, value);
        }

        static IPBox()
        {
            IPAddressProperty.Changed.AddClassHandler<IPBox>(OnIPAddressChanged);
            CaptionProperty.OverrideDefaultValue<IPBox>("IP Address");
        }

        private static void OnIPAddressChanged(IPBox userControl, AvaloniaPropertyChangedEventArgs args)
        {
            var val = args.NewValue as string;
            if (val != null && userControl.txtboxFirstPart != null)
            {
                string[] splitValues = val.Split('.');
                userControl.txtboxFirstPart.Text = splitValues[0];
                userControl.txtboxSecondPart.Text = splitValues[1];
                userControl.txtboxThridPart.Text = splitValues[2];
                userControl.txtboxFourthPart.Text = splitValues[3];
                userControl.IPAddress = val;
            }
        }

        private bool focusMoved = false;

        private static void TextboxTextCheck(object sender)
        {
            TextBox txtbox = (TextBox)sender;
            txtbox.Text = GetNumberFromString(txtbox.Text ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(txtbox.Text))
            {
                if (Convert.ToInt32(txtbox.Text) > 255)
                    txtbox.Text = "255";
                else if (Convert.ToInt32(txtbox.Text) < 0)
                    txtbox.Text = "0";
            }
            txtbox.CaretIndex = txtbox.Text.Length;
        }

        private static string GetNumberFromString(string str)
        {
            StringBuilder numberBuilder = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsNumber(c))
                    numberBuilder.Append(c);
            }
            return numberBuilder.ToString();
        }

        void UpdateIPAddress(object sender)
        {
            TextBox txtbox = (TextBox)sender;
            if (txtbox != null && txtbox.IsLoaded)
            {
                if (txtbox.Text != String.Empty)
                {
                    IPAddress = txtboxFirstPart.Text + "." + txtboxSecondPart.Text + "." + txtboxThridPart.Text + "." + txtboxFourthPart.Text;
                }
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateIPAddress(sender);
        }

        private void txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextboxTextCheck(sender);
            UpdateIPAddress(sender);
        }

        private void txtboxFirstPart_PreviewKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                txtboxSecondPart.Focus();
                txtboxSecondPart.Text = String.Empty;
                focusMoved = true;
            }
            else
            {
                focusMoved = false;
            }
        }

        private void txtboxSecondPart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal && !focusMoved)
            {
                txtboxThridPart.Focus();
                txtboxThridPart.Text = String.Empty;
                focusMoved = true;
            }
            else
            {
                focusMoved = false;
            }
        }

        private void txtboxThridPart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                txtboxFourthPart.Focus();
                txtboxFourthPart.Text = String.Empty;
            }
        }
    }
}
