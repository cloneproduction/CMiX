// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Presentation.ViewModels.Services
{
    public class MaterialManager : PrefabManager, IBeatable
    {
        public MaterialManager(PrefabManagerModel prefabManagerModel) : base(prefabManagerModel)
        {

        }

        public MasterBeat MasterBeat { get; set; }

        public override IPrefab CreatePrefab()
        {
            return CreatePrefab(new MaterialModel());
        }

        public override IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            Material prefab = null;

            if(prefabModel is MaterialModel materialModel)
            {
                prefab = new Material(materialModel);
                SelectedItem = prefab;
                prefab.IsSelected = true;
                Prefabs.Add(prefab);
                prefab.SetMasterBeat(MasterBeat);
            }

            return prefab;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
        }



        public override IModel GetModel()
        {
            IPrefabManagerModel modifierManagerModel = new PrefabManagerModel();
            modifierManagerModel.ID = this.ID;
            return modifierManagerModel;
        }
    }
}
