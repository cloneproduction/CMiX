// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CMiX.Studio.Behaviors
{
    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint |
                                                   NumberStyles.AllowThousands |
                                                   NumberStyles.AllowLeadingSign;
        public TextBoxInputBehavior()
        {
            this.InputMode = TextBoxInputMode.None;
            this.JustPositiveDecimalInput = false;
        }

        public TextBoxInputMode InputMode { get; set; }

        public static readonly DependencyProperty JustPositivDecimalInputProperty =
         DependencyProperty.Register("JustPositivDecimalInput", typeof(bool),
         typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(false));

        public bool JustPositiveDecimalInput
        {
            get { return (bool)GetValue(JustPositivDecimalInputProperty); }
            set { SetValue(JustPositivDecimalInputProperty, value); }
        }


        string oldValue = String.Empty;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LostFocus += AssociatedObject_LostFocus;
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            AssociatedObject.PreviewMouseRightButtonDown += AssociatedObject_PreviewMouseRightButtonDown;
            oldValue = AssociatedObject.Text;
        }

        private void AssociatedObject_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AssociatedObject.Text = oldValue;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        }


        private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
        {
            oldValue = AssociatedObject.Text;
        }


        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                if (!this.IsValidInput(textBox.Text))
                {
                    textBox.Text = oldValue;
                }
                oldValue = textBox.Text;
            }
        }


        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;
                case TextBoxInputMode.DigitInput:
                    return CheckIsDigit(input);

                case TextBoxInputMode.DecimalInput:
                    decimal d;

                    if (input.ToCharArray().Where(x => x == ',').Count() > 1)
                        return false;

                    if (input.Contains("-"))
                    {
                        if (!this.JustPositiveDecimalInput && input.IndexOf("-") == 0 && input.Length == 1)
                            return true;
                        else
                        {
                            var result = decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d);
                            return result;
                        }
                    }
                    else
                    {
                        var result = decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d);
                        return result;
                    }

                default: throw new ArgumentException("Unknown TextBoxInputMode");
            }
        }

        private bool CheckIsDigit(string text)
        {
            return text.ToCharArray().All(Char.IsDigit);
        }
    }

    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput
    }
}
