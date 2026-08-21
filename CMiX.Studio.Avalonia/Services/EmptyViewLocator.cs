// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Services
{
    // StrongViewLocator is abstract, so MainWindowDialogManager needs a concrete instance even
    // though nothing currently needs a view registered - popups are Flyouts declared directly in
    // XAML instead of dialog-service windows, and the message-box/file-dialog helpers used
    // elsewhere go through DialogFactory, not this locator.
    public class EmptyViewLocator : StrongViewLocator
    {
    }
}
