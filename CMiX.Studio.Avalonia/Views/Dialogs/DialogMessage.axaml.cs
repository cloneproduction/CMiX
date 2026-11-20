// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace CMiX.Studio.Avalonia.Views.Dialogs
{
    // The one message window of the app: the frame of the menus, an icon, a text and a row of buttons.
    // The window closes with the result of the button.
    public partial class DialogMessage : Window
    {
        private readonly MessageDialogOptions options;

        public DialogMessage() : this(MessageDialogOptions.Notice(string.Empty, "Warning"))
        {
        }

        public DialogMessage(MessageDialogOptions options)
        {
            this.options = options;
            InitializeComponent();

            MessageIcon.IconKey = options.IconKey;
            TitleText.Text = options.Title;
            TitleText.IsVisible = !string.IsNullOrEmpty(options.Title);
            MessageText.Text = options.Text;

            foreach (var item in options.Buttons)
            {
                var button = new Button
                {
                    Name = $"{item.Result}Button",
                    Content = item.Caption,
                    MinWidth = 100,
                    Margin = new Thickness(4),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                var result = item.Result;
                button.Click += (_, _) => Close(result);
                ButtonRow.Children.Add(button);
            }
        }

        public static Task<MessageDialogResult> ShowAsync(Window owner, MessageDialogOptions options) =>
            new DialogMessage(options).ShowDialog<MessageDialogResult>(owner);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter && options.Buttons.FirstOrDefault(button => button.IsDefault) is { } defaultButton)
                Close(defaultButton.Result);
            else if (e.Key == Key.Escape)
                Close(options.Buttons.FirstOrDefault(button => button.IsCancel)?.Result ?? MessageDialogResult.Cancel);
            else
                return;

            e.Handled = true;
        }
    }
}
