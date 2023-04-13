// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Modifiers;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.BaseControl;

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
