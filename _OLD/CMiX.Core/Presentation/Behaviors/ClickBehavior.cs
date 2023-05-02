// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace CMiX.Core.Presentation.Behaviors
{
    public class ClickBehavior : Behavior<Control>
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public ClickBehavior()
        {
            _timer.Interval = TimeSpan.FromSeconds(0.2);
            _timer.Tick += Timer_Tick;
        }

        public static readonly DependencyProperty ClickCommandPropery =
             DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(ClickBehavior));
        public ICommand ClickCommand
        {
            get => (ICommand)GetValue(ClickCommandPropery);
            set => SetValue(ClickCommandPropery, value);
        }

        public static readonly DependencyProperty DoubleClickCommandPropery =
            DependencyProperty.Register(nameof(DoubleClickCommand), typeof(ICommand), typeof(ClickBehavior));
        public ICommand DoubleClickCommand
        {
            get => (ICommand)GetValue(DoubleClickCommandPropery);
            set => SetValue(DoubleClickCommandPropery, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }
        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            ClickCommand?.Execute(null);
        }
        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _timer.Stop();
                DoubleClickCommand?.Execute(null);
            }
            else
            {
                _timer.Start();
            }
        }
    }
}
