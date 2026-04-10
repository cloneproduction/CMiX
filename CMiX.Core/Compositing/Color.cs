// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Compositing
{
    public class Color : GenericValue<string>, IPrefab
    {
        public Color(PrefabService prefabService, 
                     ControlMessenger controlMessenger, 
                     MessageFactory messageFactory, 
                     ControlActivationService activationService )
            : base(controlMessenger, 
                   messageFactory, 
                   activationService)
        {
            PrefabService = prefabService;
        }
        public PrefabService PrefabService { get; set; }
    }
}
