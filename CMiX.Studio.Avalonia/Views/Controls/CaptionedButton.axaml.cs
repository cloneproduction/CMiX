// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class CaptionedButton : CaptionedUserControl
    {
        public CaptionedButton()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<CaptionedButton, ICommand>(nameof(Command));
        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<CaptionedButton, object>(nameof(CommandParameter));
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}
