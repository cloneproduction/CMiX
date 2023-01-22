//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using System.ComponentModel;
//using CMiX.Core.Presentation.ViewModels;
//using CMiX.Core.Presentation.ViewModels.Dialogs;
//using CMiX.Core.Presentation.Views;
//using MvvmDialogs.DialogTypeLocators;

//namespace CMiX.Studio.Views.Dialogs
//{
//    public class CustomTypeLocator : IDialogTypeLocator
//    {
//        public Type Locate(INotifyPropertyChanged viewModel)
//        {
//            if (viewModel is Server)
//                return typeof(MessengerSettingsWindow);
//            else if (viewModel is ModalDialog)
//                return typeof(CustomWindowDialog);
//            else if (viewModel is Material)
//                return typeof(ColorSelectorDialog);
//            else if (viewModel is SamplerState)
//                return typeof(ColorSelectorDialog);
//            else if (viewModel is ColorSelector)
//                return typeof(ColorSelectorDialog);
//            else
//                throw new Exception("Dialog type is not defined.");
//        }
//    }
//}
