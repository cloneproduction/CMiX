// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentations.Modifiers.Transform;

namespace CMiX.Core.Presentations.Components
{
    public class EntityModel : IComponentModel, IPrefabModel
    {
        public EntityModel()
        {
            ID = Guid.NewGuid();

            TransformSRT = new TransformSRTModel();
            ModifierManager = new ModifierManagerModel();
            Mesh = new MeshModel();
            MaterialManager = new PrefabManagerModel();
        }

        public EntityModel(Guid id) : this()
        {
            ID = id;
        }

        public string Name { get; set; }
        public Guid ID { get; set; }


        public TransformSRTModel TransformSRT { get; internal set; }
        public MeshModel Mesh { get; internal set; }
        public PrefabManagerModel MaterialManager { get; internal set; }
        public ModifierManagerModel ModifierManager { get; internal set; }
    }
}
