// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace CMiX.Core.Presentation.Behaviors
{
    public static class TreeViewItemClickBehavior
    {
        #region private timer 

        public static readonly DependencyProperty ClickWaitTimer = DependencyProperty.RegisterAttached("Timer", typeof(DispatcherTimer), typeof(TreeViewItemClickBehavior));

        private static DispatcherTimer GetClickWaitTimer(DependencyObject obj)
        {
            return (DispatcherTimer)obj.GetValue(ClickWaitTimer);
        }

        private static void SetClickWaitTimer(DependencyObject obj, DispatcherTimer timer)
        {
            obj.SetValue(ClickWaitTimer, timer);
        }

        #endregion

        #region single click dependency properties 

        public static ICommand GetSingleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SingleClickCommand);
        }

        public static void SetSingleClickCommand(DependencyObject obj, ICommand command)
        {
            obj.SetValue(SingleClickCommand, command);
        }

        public static readonly DependencyProperty SingleClickCommand = DependencyProperty.RegisterAttached("SingleClickCommand",
            typeof(ICommand), typeof(TreeViewItemClickBehavior),
            new UIPropertyMetadata(null, CommandChanged));

        public static object GetSingleClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(SingleClickCommandParameter);
        }

        public static void SetSingleClickCommandParameter(DependencyObject obj, ICommand command)
        {
            obj.SetValue(SingleClickCommandParameter, command);
        }

        public static readonly DependencyProperty SingleClickCommandParameter = DependencyProperty.RegisterAttached("SingleClickCommandParameter",
            typeof(object), typeof(TreeViewItemClickBehavior));

        #endregion

        #region double click dependency properties 

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickCommand);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand command)
        {
            obj.SetValue(DoubleClickCommand, command);
        }

        public static readonly DependencyProperty DoubleClickCommand = DependencyProperty.RegisterAttached("DoubleClickCommand",
            typeof(ICommand), typeof(TreeViewItemClickBehavior),
            new UIPropertyMetadata(null, CommandChanged));

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(DoubleClickCommandParameter);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, ICommand command)
        {
            obj.SetValue(DoubleClickCommandParameter, command);
        }

        public static readonly DependencyProperty DoubleClickCommandParameter = DependencyProperty.RegisterAttached("DoubleClickCommandParameter",
            typeof(object), typeof(TreeViewItemClickBehavior));




        public static ICommand GetPreviewMouseUpCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PreviewMouseUpCommand);
        }

        public static void SetPreviewMouseUpCommand(DependencyObject obj, ICommand command)
        {
            obj.SetValue(PreviewMouseUpCommand, command);
        }

        public static readonly DependencyProperty PreviewMouseUpCommand = DependencyProperty.RegisterAttached("PreviewMouseUpCommand",
        typeof(ICommand), typeof(TreeViewItemClickBehavior),
        new UIPropertyMetadata(null, PreviewMouseUpCommandChange));

        public static object GetPreviewMouseUpCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(PreviewMouseUpCommandParameter);
        }

        public static void SetPreviewMouseUpCommandParameter(DependencyObject obj, ICommand command)
        {
            obj.SetValue(PreviewMouseUpCommandParameter, command);
        }

        public static readonly DependencyProperty PreviewMouseUpCommandParameter = DependencyProperty.RegisterAttached("PreviewMouseUpCommandParameter",
            typeof(object), typeof(TreeViewItemClickBehavior));
        #endregion

        private static void PreviewMouseUpCommandChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Control;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior can be attached to a TextBox item only.");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.PreviewMouseUp += OnPreviewMouseUp;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.PreviewMouseUp -= OnPreviewMouseUp;
            }
        }

        private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(PreviewMouseUpCommand);
            if (command != null)
            {
                command.Execute(e);
            }
            e.Handled = true;
        }

        private static void CommandChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var targetElement = sender as TreeViewItem;

            if (targetElement != null)
            {
                
                //remove any existing handlers 
                targetElement.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(element_MouseLeftButtonDown));
                //use AddHandler to be able to listen to handled events 
                targetElement.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(element_MouseLeftButtonDown), true);

                //if the timer has not been created then do so 
                var timer = GetClickWaitTimer(targetElement);

                if (timer == null)
                {
                    timer = new DispatcherTimer() { IsEnabled = false };
                    timer.Interval = TimeSpan.FromSeconds(0.2);
                    timer.Tick += (s, args) =>
                    {
                        //if the interval has been reached without a second click then execute the SingClickCommand  
                        timer.Stop();

                        var command = targetElement.GetValue(SingleClickCommand) as ICommand;
                        var commandParameter = targetElement.GetValue(SingleClickCommandParameter);

                        if (command != null)
                        {
                            if (command.CanExecute(e))
                            {
                                command.Execute(commandParameter);
                            }
                        }
                    };

                    SetClickWaitTimer(targetElement, timer);
                }
            }
        }

        private static void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var targetElement = sender as TreeViewItem;
            if (targetElement != null)
            {
                var timer = GetClickWaitTimer(targetElement);

                //if the timer is enabled there has already been one click within the interval and this is a second click so  
                //stop the timer and execute the DoubleClickCommand 
                if (timer.IsEnabled)
                {
                    timer.Stop();

                    var command = targetElement.GetValue(DoubleClickCommand) as ICommand;
                    var commandParameter = targetElement.GetValue(DoubleClickCommandParameter);

                    if (command != null)
                    {
                        if (command.CanExecute(e))
                        {
                            command.Execute(commandParameter);
                        }
                    }
                }
                else
                {
                    timer.Start();
                }
            }
        }
    }

}
