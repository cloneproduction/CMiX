// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.Views;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using MvvmDialogs.FrameworkDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CustomFrameworkDialogFactory : DefaultFrameworkDialogFactory, IDialogFactory
    {
        public IWindow Create(Type dialogType)
        {
            if (dialogType == typeof(MessengerSettingsWindow))
                return new MessageSettingsDialog();
            else if (dialogType == typeof(TaskEditor))
                return new TaskEditorDialog();
            else
                return new CustomDialog();

        }
    }
}
