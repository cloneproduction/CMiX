// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Core.Presentation.Controls
{
    public partial class IPBox : UserControl
    {
        public IPBox()
        {
            InitializeComponent();
        }

        #region PROPERTIES
        public static readonly DependencyProperty IPAddressProperty =
        DependencyProperty.Register("IPAddress", typeof(string), typeof(IPBox), new FrameworkPropertyMetadata(null, PropertyChangedCallback));
        [Bindable(true)]
        public string IPAddress
        {
            get => (string)GetValue(IPAddressProperty);
            set => SetValue(IPAddressProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            IPBox userControl = ((IPBox)dependencyObject);
            var val = (string)args.NewValue;
            if (val != null)
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
        #endregion

        #region METHODS
        private static void TextboxTextCheck(object sender)
        {
            TextBox txtbox = (TextBox)sender;
            txtbox.Text = GetNumberFromString(txtbox.Text);
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
        #endregion

        #region EVENTS
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateIPAddress(sender);
        }

        private void txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextboxTextCheck(sender);
            UpdateIPAddress(sender);
        }

        private void txtboxFirstPart_PreviewKeyDown(object sender, KeyEventArgs e)
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
        #endregion
    }
}
