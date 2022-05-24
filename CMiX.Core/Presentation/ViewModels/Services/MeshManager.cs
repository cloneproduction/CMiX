// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Presentation.ViewModels.Services
{
    public class MeshManager : PrefabManager, IBeatable
    {
        public MeshManager(PrefabManagerModel prefabManagerModel) : base(prefabManagerModel)
        {

        }

        public override IPrefab CreatePrefab()
        {
            return CreatePrefab(new MeshModel());
        }

        public override IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            Mesh prefab = null;

            if (prefabModel is MeshModel materialModel)
            {
                prefab = new Mesh(materialModel);
                SelectedItem = prefab;
                prefab.IsSelected = true;
                Prefabs.Add(prefab);
                prefab.SetMasterBeat(MasterBeat);
            }

            return prefab;
        }

        public MasterBeat MasterBeat { get; set; }

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
