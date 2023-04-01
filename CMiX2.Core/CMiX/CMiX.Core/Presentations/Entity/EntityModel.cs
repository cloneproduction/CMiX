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

        public StringValueModel Name { get; set; }
        public Guid ID { get; set; }


        public TransformSRTModel TransformSRT { get; internal set; }
        public MeshModel Mesh { get; internal set; }
        public PrefabManagerModel MaterialManager { get; internal set; }
        public ModifierManagerModel ModifierManager { get; internal set; }
        public BooleanValueModel IsSelected { get; internal set; }
        public BooleanValueModel IsRenaming { get; internal set; }
    }
}
