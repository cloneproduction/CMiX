// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
            Value = m.Value;
            PrefabService.FromModel(m.PrefabService);
        }
    }
}
