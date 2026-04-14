// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Compositing
{
    public class Color : GenericValue<string>, IPrefab
    {
        public Color(PrefabService prefabService, 
                     ControlMessenger controlMessenger, 
                     MessageFactory messageFactory, 
                     ControlActivationService activationService,
                     UndoManager undoManager)
            : base(controlMessenger, 
                   messageFactory, 
                   activationService, 
                   undoManager)
        {
            PrefabService = prefabService;
            Debug.WriteLine($"Color UndoManager={UndoManager != null}, IsActive={IsActive}");
            Console.WriteLine($"Color UndoManager={UndoManager != null}, IsActive={IsActive}");
        }
        public PrefabService PrefabService { get; set; }
        protected override IControlModel CaptureModel() => ToModel();

        public IControlModel ToModel() => new ColorModel
        {
            ID = ID,
            Value = Value,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ColorModel)model;
            ID = m.ID;
            IsActive = false;
            Value = m.Value;
            IsActive = true;
            Debug.WriteLine($"Color.FromModel called, ID={ID}");
            PrefabService.FromModel(m.PrefabService);
        }
    }
}
