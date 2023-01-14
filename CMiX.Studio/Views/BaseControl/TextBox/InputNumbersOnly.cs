// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Core.Presentation.Controls
{
    public class InputNumbersOnly : DependencyObject
    {
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(InputNumbersOnly),
            new PropertyMetadata(false, ActivePropertyChanged));

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetActive(DependencyObject @object)
        {
            return (bool)@object.GetValue(ActiveProperty);
        }

        public static void SetActive(DependencyObject @object, bool value)
        {
            @object.SetValue(ActiveProperty, value);
        }


        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox textBox = d as TextBox;
                if ((e.NewValue as bool?).GetValueOrDefault(false))
                {
                    textBox.PreviewTextInput += OnTextInput;
                    textBox.PreviewKeyDown += OnPreviewKeyDown;
                    DataObject.AddPastingHandler(textBox, OnPaste);
                }
                else
                {
                    textBox.PreviewTextInput -= OnTextInput;
                    textBox.PreviewKeyDown -= OnPreviewKeyDown;
                    DataObject.RemovePastingHandler(textBox, OnPaste);
                }
            }
        }

        private static void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(c => !char.IsDigit(c))) { e.Handled = true; }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = Convert.ToString(e.DataObject.GetData(DataFormats.Text)).Trim();
                if (text.Any(c => !char.IsDigit(c))) { e.CancelCommand(); }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
