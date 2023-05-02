// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels;
using CMiX.Core.Transformation;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Compositing
{
    public class EntityModel : IPrefabModel
    {
        public EntityModel()
        {
            ID = Guid.NewGuid();

            TransformSRT = new TransformSRTModel();
            ModifierManager = new ModifierManagerModel();
            Mesh = new MeshModel();
            MaterialManager = new PrefabManagerModel();
            Name = new StringValueModel("Entity " + ID.ToString());
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
        }

        public EntityModel(Guid id) : this()
        {
            ID = id;
        }

        public Guid ID { get; set; }
        public StringValueModel Name { get; set; }
        public TransformSRTModel TransformSRT { get; set; }
        public MeshModel Mesh { get; set; }
        public PrefabManagerModel MaterialManager { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
