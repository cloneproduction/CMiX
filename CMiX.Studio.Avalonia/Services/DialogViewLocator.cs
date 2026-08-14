// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Servers;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace CMiX.Studio.Avalonia.Services
{
    // Maps dialog view models to their windows for HanumanInstitute.MvvmDialogs.
    public class DialogViewLocator : StrongViewLocator
    {
        public DialogViewLocator()
        {
            Register<ServerManager, Views.ServerCreationWindow>();
        }
    }
}
