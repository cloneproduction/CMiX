// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Windows;
using Button = System.Windows.Controls.Button;
using System.Windows.Forms;
using System.Windows.Interactivity;

namespace CMiX.Core.Presentation.Behaviors
{
    public class FolderDialogBehavior : Behavior<Button>
    {
        public string SetterName { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && AssociatedObject.DataContext != null)
            {
                var propertyInfo = AssociatedObject.DataContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && p.CanWrite)
                    .First(p => p.Name.Equals(SetterName));

                propertyInfo.SetValue(AssociatedObject.DataContext, dialog.SelectedPath, null);
            }
        }
    }
}
